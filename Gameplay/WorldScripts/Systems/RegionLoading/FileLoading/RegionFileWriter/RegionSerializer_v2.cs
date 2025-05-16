using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.ChunkSerializers;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.RegionSerialization;


namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.RegionFileWriter;

public class RegionSerializer_v2 : IRegionSerializer
{
    private const int FORMAT_VERSION = 2;
    private const int REGION_TABLE_ENTRY_SIZE = sizeof(int) + sizeof(long) * 2;
    private SerializerSelector selector = new SerializerSelector();

    public void Save(FileStream stream, RegionCoord regionCoord, RegionData data)
    {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.Default, leaveOpen: true);
        using var reader = new BinaryReader(stream, System.Text.Encoding.Default, leaveOpen: true);

        // Load existing index
        var index = new List<RegionIndexEntry>();
        if (stream.Length >= sizeof(int) * 3)
        {
            stream.Position = 0;
            int version = reader.ReadInt32();
            if (version != FORMAT_VERSION)
                throw new InvalidDataException($"Expected version {FORMAT_VERSION}, found {version}");

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                index.Add(new RegionIndexEntry(
                    reader.ReadInt32(), // X
                    reader.ReadInt32(), // Z
                    reader.ReadInt64(), // ptr
                    reader.ReadInt64()  // len
                ));
            }
        }

        // Save region to block
        byte[] block = SerializeRegionBlock(regionCoord, data);
        long blockOffset = stream.Length;

        // Append
        stream.Position = blockOffset;
        writer.Write(block);

        // Update index
        bool replaced = false;
        for (int i = 0; i < index.Count; i++)
        {
            if (index[i].X == regionCoord.X && index[i].Z == regionCoord.Z)
            {
                index[i] = new RegionIndexEntry(regionCoord.X, regionCoord.Z, blockOffset, block.Length);
                replaced = true;
                break;
            }
        }
        if (!replaced)
            index.Add(new RegionIndexEntry(regionCoord.X, regionCoord.Z, blockOffset, block.Length));

        // Rewrite header + index
        stream.Position = 0;
        writer.Write(FORMAT_VERSION);
        writer.Write(index.Count);
        foreach (var e in index)
        {
            writer.Write(e.X);
            writer.Write(e.Z);
            writer.Write(e.Offset);
            writer.Write(e.Length);
        }
    }

    public bool Load(FileStream stream, RegionCoord target, out RegionData data)
    {
        data = default!;
        using var reader = new BinaryReader(stream, System.Text.Encoding.Default, leaveOpen: true);

        if (stream.Length < sizeof(int) * 3) return false;
        stream.Position = 0;
        int version = reader.ReadInt32();
        if (version != FORMAT_VERSION) return false;

        int count = reader.ReadInt32();
        RegionIndexEntry? found = null;
        for (int i = 0; i < count; i++)
        {
            var entry = new RegionIndexEntry(
                reader.ReadInt32(), reader.ReadInt32(),
                reader.ReadInt64(), reader.ReadInt64());
            if (entry.X == target.X && entry.Z == target.Z)
                found = entry;
        }
        if (found == null) return false;
        if (found.Length <= 0 || found.Offset < 0 || found.Offset + found.Length > stream.Length)
            throw new InvalidDataException($"Invalid region block pointers: offset={found.Offset}, length={found.Length}, streamLength={stream.Length}");

        stream.Position = found.Offset;
        byte[] block = reader.ReadBytes((int)found.Length);
        data = DeserializeRegionBlock(block, out _);
        return true;
    }

    private byte[] SerializeRegionBlock(RegionCoord region, RegionData data)
    {
        int chunkCount = data.Chunks.Length;
        var blobs = new byte[chunkCount][];
        var algos = new int[chunkCount];
        for (int i = 0; i < chunkCount; i++)
        {
            blobs[i] = selector.Serialize(data.Chunks[i], out algos[i]).ToArray();
        }

        const int headerSize = sizeof(int) * 3;
        int tableSize = REGION_TABLE_ENTRY_SIZE * chunkCount;
        long dataOffset = headerSize + tableSize;

        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);

        // Header
        w.Write(region.X);
        w.Write(region.Z);
        w.Write(chunkCount);

        // Table
        long ptr = dataOffset;
        for (int i = 0; i < chunkCount; i++)
        {
            w.Write(algos[i]);
            w.Write(ptr);
            w.Write((long)blobs[i].Length);

            if (ptr < 0 || blobs[i].Length < 0)
                throw new InvalidDataException($"Overflow while writing table: chunk {i}, ptr={ptr}, len={blobs[i].Length}");

            ptr += blobs[i].Length;
        }

        // Data blobs
        foreach (var b in blobs) w.Write(b);
        return ms.ToArray();
    }

    private RegionData DeserializeRegionBlock(byte[] block, out RegionCoord region)
    {
        int blockLen = block.Length;
        using var ms = new MemoryStream(block);
        using var r = new BinaryReader(ms);

        int rx = r.ReadInt32();
        int rz = r.ReadInt32();
        int count = r.ReadInt32();
        region = new RegionCoord(rx, rz);

        long tableStart = sizeof(int) * 3;
        long tableSize = (long)count * REGION_TABLE_ENTRY_SIZE;

        if (tableStart + tableSize > blockLen)
            throw new InvalidDataException($"Chunk table overruns block: tableStart={tableStart}, tableSize={tableSize}, blockLen={blockLen}");

        var chunks = new ChunkData[count];
        for (int i = 0; i < count; i++)
        {
            long entryOffset = tableStart + (long)i * REGION_TABLE_ENTRY_SIZE;
            if (entryOffset < 0 || entryOffset + REGION_TABLE_ENTRY_SIZE > blockLen)
                throw new InvalidDataException($"Entry {i} offset invalid: entryOffset={entryOffset}, tableSize={tableSize}, blockLen={blockLen}");

            ms.Position = entryOffset;
            int algo = r.ReadInt32();
            long ptr = r.ReadInt64();
            long len = r.ReadInt64();

            if (ptr < tableStart + tableSize)
                throw new InvalidDataException($"Chunk data {i} starts inside header/table: ptr={ptr}, minDataStart={tableStart + tableSize}");

            if (ptr < 0 || len < 0 || ptr + len > blockLen)
                throw new InvalidDataException($"Chunk data {i} out of bounds: ptr={ptr}, len={len}, blockLen={blockLen}");

            ms.Position = ptr;
            byte[] blob = r.ReadBytes((int)len);
            if (blob.Length != len)
                throw new InvalidDataException($"Unexpected read size for chunk {i}: expected {len}, got {blob.Length}");

            chunks[i] = selector.Deserialize(algo, blob);

            if (chunks[i] == null)
                throw new InvalidDataException($"Deserialized chunk {i} is null after algorithm {algo} decode.");
        }

        return new RegionData(chunks);
    }

    private record RegionIndexEntry(int X, int Z, long Offset, long Length);
}
