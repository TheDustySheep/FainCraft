using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Chunks;
using FainCraft.Gameplay.WorldScripts.Data.Regions;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Files.ChunkSerializers;
using System.Runtime.InteropServices;
using System.Text;
using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Files.RegionFileWriter;

public class RegionSerializer_v3 : IRegionSerializer
{
    static readonly Encoding ENCODING = Encoding.UTF8; 
    const uint VERSION = 3;
    private ChunkEncoder selector = new ChunkEncoder();

    public LoadResult Load(LoadRequest request)
    {
        Stream stream = request.Stream;
        using var reader = new BinaryReader(request.Stream, ENCODING, leaveOpen: true);
        var header = FileHeader.Read(reader);

        if (!header.CompareIsValid(request.SaveCoord))
            throw new InvalidDataException("Region file version or size mismatch");

        var tableHeader = new ChunkTable()
        {
            Stream = stream,
            Location = stream.Position,
        };

        var tableRows = tableHeader.ReadRows(reader);

        var result = new LoadResult
        {
            Regions = new Dictionary<RegionCoord, RegionData?>()
        };

        foreach (var rCoord in request.RegionCoords)
        {
            RegionData? data = LoadRegion(tableRows, reader, stream, rCoord);
            result.Regions.Add(rCoord, data);
        }

        return result;
    }

    private RegionData? LoadRegion(
        List<ChunkTable.TableRow> tableRows, 
        BinaryReader reader,
        Stream stream,
        RegionCoord rCoord)
    {
        ChunkData[] cDatas = new ChunkData[REGION_TOTAL_CHUNKS];
        for (uint y = 0; y < REGION_TOTAL_CHUNKS; y++)
        {
            SaveCoordOffset offset = new SaveCoordOffset(rCoord, y);

            // Try to find the existingRow row
            int existingIndex = tableRows.FindIndex(r =>
                r.SaveCoord == offset &&
                r.Encoding >= 0
            );

            // Missing chunk
            if (existingIndex == -1)
                return null;

            // Data index
            var row = tableRows[existingIndex];
            stream.Position = row.Location;

            // Load data
            ReadOnlySpan<byte> span = reader.ReadBytes(row.Length);
            cDatas[y] = selector.Decode(span, row.Encoding);
        }

        return new RegionData(cDatas);
    }

    public SaveResult Save(SaveRequest request)
    {
        Stream stream = request.Stream;

        if (stream.Length == 0)
            return SaveNewFile(request);
        else
            return UpdateSaveFile(request);
    }

    public SaveResult UpdateSaveFile(SaveRequest request)
    {
        Stream stream = request.Stream;
        using var writer = new BinaryWriter(stream, ENCODING, leaveOpen: true);
        using var reader = new BinaryReader(stream, ENCODING, leaveOpen: true);

        var header = FileHeader.Read(reader);
        if (!header.CompareIsValid(request.SaveCoord))
            throw new InvalidDataException($"SaveAsync coord was not valid for this region Request: ({request.SaveCoord.X}, {request.SaveCoord.Z}) Files: ({header.SaveCoordX}, {header.SaveCoordZ})");

        // Header Table Data
        var chunkTable = new ChunkTable
        {
            Location = stream.Position,
            Stream = stream,
        };

        var chunkTableRows = chunkTable.ReadRows(reader);

        UpdateChunkData(request, stream, writer, chunkTable, chunkTableRows);

        return new SaveResult
        {
            Regions = request.Regions.Keys.ToDictionary(c => c, _ => true)
        };
    }

    public SaveResult SaveNewFile(SaveRequest request)
    {
        Stream stream = request.Stream;
        using var writer = new BinaryWriter(stream, ENCODING, leaveOpen: true);
        using var reader = new BinaryReader(stream, ENCODING, leaveOpen: true);

        // General Headers
        var header = new FileHeader
        {
            Version = VERSION,
            SaveCoordX = request.SaveCoord.X,
            SaveCoordZ = request.SaveCoord.Z,
            SaveRegionSizeXZ = SaveCoord.REGION_SIZE_XZ,
            SaveRegionSizePY = REGION_POS_CHUNKS,
            SaveRegionSizeNY = REGION_NEG_CHUNKS,
            LastModifiedUTC = DateTime.UtcNow.Ticks,
        };
        header.Write(writer);

        // Header Table Data
        var chunkTable = new ChunkTable
        {
            Location = stream.Position,
            Stream = stream,
        };

        var chunkTableRows = new List<ChunkTable.TableRow>(SaveCoord.REGION_AREA * REGION_TOTAL_CHUNKS);

        for (uint x = 0; x < SaveCoord.REGION_SIZE_XZ; x++)
        {
            for (uint z = 0; z < SaveCoord.REGION_SIZE_XZ; z++)
            {
                for (uint y = 0; y < REGION_TOTAL_CHUNKS; y++)
                {
                    chunkTableRows.Add(new ChunkTable.TableRow()
                    {
                        SaveCoord = new SaveCoordOffset(x, y, z),
                        Encoding = -1, // No Data
                        Length   =  0,
                        Location =  0,
                    });
                }
            }
        }

        UpdateChunkData(request, stream, writer, chunkTable, chunkTableRows);

        return new SaveResult
        {
            Regions = request.Regions.Keys.ToDictionary(c => c, _ => true)
        };
    }

    private void UpdateChunkData(SaveRequest request, Stream stream, BinaryWriter writer, ChunkTable chunkTable, List<ChunkTable.TableRow> chunkTableRows)
    {
        foreach (var kv in request.Regions)
        {
            var rCoord = kv.Key;
            var region = kv.Value;

            for (uint y = 0; y < REGION_TOTAL_CHUNKS; y++)
            {
                SaveCoordOffset offset = new SaveCoordOffset(rCoord, y);

                var span = selector.Encode(region.Chunks[y], out byte algorithm);

                var row = new ChunkTable.TableRow
                {
                    SaveCoord = offset,
                    Encoding  = algorithm,
                    Length    = span.Length
                };

                // Find a free spot
                long dataLoc = chunkTable.UpdateOrAddRow(chunkTableRows, row);

                // Write data blob
                stream.Position = dataLoc;
                stream.Write(span);
            }
        }
        chunkTable.WriteRows(writer, chunkTableRows);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileHeader
    {
        public required uint Version;
        public required  int SaveCoordX;
        public required  int SaveCoordZ;

        public required uint SaveRegionSizeXZ;
        public required  int SaveRegionSizePY;
        public required  int SaveRegionSizeNY;

        public required long LastModifiedUTC;

        public readonly void Write(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(SaveCoordX);
            writer.Write(SaveCoordZ);
            writer.Write(SaveRegionSizeXZ);
            writer.Write(SaveRegionSizePY);
            writer.Write(SaveRegionSizeNY);
            writer.Write(LastModifiedUTC);
        }

        public static FileHeader Read(BinaryReader reader) => new()
        {
            Version          = reader.ReadUInt32(),
            SaveCoordX       = reader.ReadInt32 (),
            SaveCoordZ       = reader.ReadInt32 (),
            SaveRegionSizeXZ = reader.ReadUInt32(),
            SaveRegionSizePY = reader.ReadInt32 (),
            SaveRegionSizeNY = reader.ReadInt32 (),
            LastModifiedUTC  = reader.ReadInt64 (),
        };

        public readonly bool CompareIsValid(SaveCoord coord)
        {
            return 
                Version          == VERSION &&
                SaveCoordX       == coord.X &&
                SaveCoordZ       == coord.Z &&
                SaveRegionSizeXZ == SaveCoord.REGION_SIZE_XZ &&
                SaveRegionSizePY == REGION_POS_CHUNKS &&
                SaveRegionSizeNY == REGION_NEG_CHUNKS;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ChunkTable
    {
        public required long Location;
        public required Stream Stream;

        public const long Size = TableRow.Size * SaveCoord.REGION_AREA * REGION_TOTAL_CHUNKS;

        public readonly void WriteRows(BinaryWriter writer, List<TableRow> rows)
        {
            if (rows.Count != SaveCoord.REGION_AREA * REGION_TOTAL_CHUNKS)
                throw new InvalidDataException($"Number of rows should equal constant length {rows.Count} / {SaveCoord.REGION_AREA * REGION_TOTAL_CHUNKS}");

            Stream.Position = Location;
            for (int i = 0; i < rows.Count; i++)
                rows[i].Write(writer);
        }

        public readonly List<TableRow> ReadRows(BinaryReader reader)
        {
            List<TableRow> rows = new (SaveCoord.REGION_AREA * REGION_TOTAL_CHUNKS);

            Stream.Position = Location;
            for (int i = 0; i < SaveCoord.REGION_AREA * REGION_TOTAL_CHUNKS; i++)
                rows.Add(TableRow.Read(reader));

            return rows;
        }

        public long UpdateOrAddRow(
            List<TableRow> rows,
            TableRow newRow
            )
        {
            long minDataLocation = Location + Size;

            // Try to find the existingRow row
            int existingIndex = rows.FindIndex(r => 
                r.SaveCoord == newRow.SaveCoord
            );

            if (existingIndex < 0)
                throw new InvalidDataException("Table row could not be located");
            else
            {
                TableRow existingRow = rows[existingIndex];

                // Is this a valid row and can we use it?
                if (existingRow.Encoding >= 0 &&                // Actually exists
                    existingRow.Location >= minDataLocation &&  // Location is valid
                    existingRow.Length   >= newRow.Length)      // Size is large enough
                {
                    newRow.Location = existingRow.Location;
                    rows[existingIndex] = newRow;
                    return newRow.Location;
                }
            }

            // Could not find a valid row to overwrite - Search for a new spot
            var list = rows
                .Where(i => 
                    i.Encoding >= 0 && 
                    i.Location >= minDataLocation && 
                    i.Length > 0)
                .OrderBy(r => r.Location);

            // Search for a gap between rows big enough for newSize
            long prevEnd = minDataLocation;
            foreach (var row in list)
            {
                long gap = row.Location - prevEnd;
                if (gap >= newRow.Length)
                {
                    newRow.Location = prevEnd;
                    rows[existingIndex] = newRow;
                    return newRow.Location;
                }
                prevEnd = row.Location + row.Length;
            }

            newRow.Location = prevEnd;
            rows[existingIndex] = newRow;
            return newRow.Location;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TableRow
        {
            public const int Size = 2 + 2 + 8 + 4; // 16 bytes

            public SaveCoordOffset SaveCoord;
            public short Encoding;
            public  long Location;
            public   int Length;

            public static TableRow Read(BinaryReader reader) => new TableRow()
            {
                SaveCoord = new SaveCoordOffset(reader.ReadUInt16()),
                Encoding  = reader.ReadInt16(),
                Location  = reader.ReadInt64(),
                Length    = reader.ReadInt32(),
            };

            public readonly void Write(BinaryWriter writer)
            {
                writer.Write(SaveCoord.Index);
                writer.Write(Encoding);
                writer.Write(Location);
                writer.Write(Length);
            }
        }
    }
}
