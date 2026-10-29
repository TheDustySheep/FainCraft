using FainCraft.Gameplay.WorldScripts.Data.ChunkDataArrays;
using FainCraft.Gameplay.WorldScripts.Data.Chunks;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using System.Buffers.Binary;
using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Files.ChunkSerializers
{
    public class RunLengthEncoder : IChunkEncoder
    {
        public ChunkDataRef Deserialize(ReadOnlySpan<byte> chunkData)
        {
            if (chunkData.Length == 0 || chunkData.Length % 8 != 0)
                throw new ArgumentException("Invalid data length. Must be a multiple of 8 bytes.");

            int runCount = chunkData.Length / 8;

            // First pass: calculate total output length
            int totalLength = 0;
            for (int i = 0; i < runCount; i++)
            {
                uint count = BinaryPrimitives.ReadUInt32LittleEndian(chunkData.Slice(i * 8 + 4, 4));
                totalLength += (int)count;
            }

            if (totalLength != CHUNK_VOLUME)
                throw new Exception($"Deserialized RLE data was not the correct length for a chunk {totalLength} / {CHUNK_VOLUME}");

            VoxelState[] result = new VoxelState[totalLength];
            int offset = 0;

            // Second pass: fill in the result array
            for (int i = 0; i < runCount; i++)
            {
                uint index = BinaryPrimitives.ReadUInt32LittleEndian(chunkData.Slice(i * 8, 4));
                uint count = BinaryPrimitives.ReadUInt32LittleEndian(chunkData.Slice(i * 8 + 4, 4));

                for (uint j = 0; j < count; j++)
                {
                    result[offset++] = new VoxelState() { Index = index };
                }
            }

            return new ChunkDataRef(new ChunkDataArray(result));
        }

        public ReadOnlySpan<byte> Serialize(ChunkDataRef chunkData)
        {
            if (chunkData.IsEmpty())
                return ReadOnlySpan<byte>.Empty;

            Span<VoxelState> voxels = new VoxelState[CHUNK_VOLUME];
            chunkData.CopyTo(voxels);

            // First pass: count unique runs
            uint? prevVoxel = null;
            int runCount = 0;

            for (int i = 0; i < voxels.Length; i++)
            {
                uint currVoxel = voxels[i].Index;
                if (currVoxel != prevVoxel)
                {
                    runCount++;
                    prevVoxel = currVoxel;
                }
            }

            uint[] indexes = new uint[runCount];
            uint[] counts = new uint[runCount];

            int arrIdx = 0;
            uint currentValue = voxels[0].Index;
            uint count = 1;

            for (int i = 1; i < voxels.Length; i++)
            {
                uint voxel = voxels[i].Index;
                if (voxel == currentValue)
                {
                    count++;
                }
                else
                {
                    indexes[arrIdx] = currentValue;
                    counts[arrIdx] = count;
                    arrIdx++;
                    currentValue = voxel;
                    count = 1;
                }
            }

            // SaveAsync last run
            indexes[arrIdx] = currentValue;
            counts[arrIdx] = count;

            // Allocate byte buffer: each uint is 4 bytes
            int totalBytes = runCount * 2 * sizeof(uint);
            byte[] buffer = new byte[totalBytes];
            Span<byte> span = buffer;

            // SaveAsync: first indexes, then counts (alternating pairs)
            int offset = 0;
            for (int i = 0; i < runCount; i++)
            {
                BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), indexes[i]);
                offset += 4;
                BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), counts[i]);
                offset += 4;
            }

            return buffer.AsSpan();
        }
    }
}
