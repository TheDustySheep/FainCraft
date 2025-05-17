using CsvHelper;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.ChunkSerializers;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.RegionSerialization;
using System.Runtime.InteropServices;
using System.Text;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.RegionFileWriter;

public class RegionSerializer_v3 : IRegionSerializer
{
    const uint VERSION = 3;
    private ChunkEncoder selector = new ChunkEncoder();

    public LoadResult Load(LoadRequest request)
    {
        var result = new LoadResult
        {
            Regions = new Dictionary<RegionCoord, RegionData?>()
        };

        // 1) Read header
        using var reader = new BinaryReader(request.Stream, Encoding.Default, leaveOpen: true);
        var header = FileHeader.Read(reader);

        if (!header.CompareIsValid(request.SaveCoord))
            throw new InvalidDataException("Region file version or size mismatch");

        // 3) Read chunk‐table metadata
        long tableLocation = request.Stream.Position;
        var table = new ChunkTable
        {
            Location = tableLocation,
            Stream = request.Stream
        };
        var allRows = table.ReadAllRows(reader);

        // 4) Initialize RegionData for each requested region
        foreach (var coord in request.RegionCoords)
        {
            var region = new RegionData();
            result.Regions[coord] = region;

            foreach (var row in allRows)
            {
                if (!row.IsValid)
                    continue;

                var rCoord = new RegionCoord(row.XPos, row.ZPos);

                if (coord != rCoord)
                    continue;

                // compute chunk-index from YPos
                int yIndex = row.YPos + WorldConstants.REGION_NEG_CHUNKS;

                // seek & read raw data
                request.Stream.Position = row.Location;
                byte[] raw = reader.ReadBytes((int)row.Length);

                // deserialize chunk
                var data = selector.Deserialize(raw, row.Encoding);

                // store in RegionData
                region.Chunks[yIndex] = data;
            }
        }

        return result;
    }


    public SaveResult Save(SaveRequest request)
    {
        Stream stream = request.Stream;
        bool isNewFile = stream.Length == 0;
        using var writer = new BinaryWriter(stream, Encoding.Default, leaveOpen: true);
        using var reader = new BinaryReader(stream, Encoding.Default, leaveOpen: true);

        // (1) Write header
        var header = new FileHeader
        {
            Version = VERSION,
            SaveCoordX = request.SaveCoord.X,
            SaveCoordZ = request.SaveCoord.Z,
            SaveRegionSizeXZ = SaveCoord.REGION_SIZE_XZ,
            SaveRegionSizePY = WorldConstants.REGION_POS_CHUNKS,
            SaveRegionSizeNY = WorldConstants.REGION_NEG_CHUNKS,
        };
        header.Write(writer);

        // (2) Prepare chunk‐table metadata
        var chunkTable = new ChunkTable
        {
            Location = stream.Position,
            Stream = stream,
        };

        // (3) If brand‐new file, reserve & write empty table once
        if (isNewFile)
        {
            var empty = new ChunkTable.TableRow
            {
                IsValid = false,
                XPos = 0,
                YPos = 0,
                ZPos = 0,
                Encoding = 0,
                Location = 0,
                Length = 0
            };

            // Write REGION_AREA * TOTAL_CHUNKS empty rows
            int totalRows = SaveCoord.REGION_AREA * WorldConstants.REGION_TOTAL_CHUNKS;
            for (int i = 0; i < totalRows; i++)
                empty.Write(writer);
        }

        // (4) Read the full table exactly once into memory
        var allRows = chunkTable.ReadAllRows(reader);

        // (5) Now serialize each chunk, updating in‐memory table and on‐disk row
        foreach (var kv in request.Regions)
        {
            var rCoord = kv.Key;
            var region = kv.Value;

            for (int y = 0; y < WorldConstants.REGION_TOTAL_CHUNKS; y++)
            {
                // Serialize chunk
                var span = selector.Serialize(region.Chunks[y], out int algorithm);

                // Find free spot using cached rows
                long dataLoc = chunkTable.FindFreeSpace(allRows, span.Length);

                // Write data blob
                stream.Position = dataLoc;
                stream.Write(span);

                // Build TableRow
                var cCoord = (ChunkCoord)rCoord;
                cCoord.Y = y - WorldConstants.REGION_NEG_CHUNKS;

                var row = new ChunkTable.TableRow
                {
                    IsValid = true,
                    XPos = cCoord.X,
                    YPos = cCoord.Y,
                    ZPos = cCoord.Z,
                    Encoding = algorithm,
                    Location = dataLoc,
                    Length = span.Length
                };

                // Update in-memory
                int xIdx = SaveCoord.Convert_Region_Coord_To_Offset(cCoord.X);
                int yIdx = cCoord.Y + WorldConstants.REGION_NEG_CHUNKS;
                int zIdx = SaveCoord.Convert_Region_Coord_To_Offset(cCoord.Z);
                int rowIdx = yIdx + xIdx * SaveCoord.REGION_SIZE_XZ + zIdx * SaveCoord.REGION_AREA;
                allRows[rowIdx] = row;

                // Overwrite just that row on disk
                chunkTable.WriteRow(writer, request.SaveCoord, cCoord, row);
            }
        }

        return new SaveResult
        {
            Regions = request.Regions.Keys.ToDictionary(c => c, _ => true)
        };
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

        public readonly void Write(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(SaveCoordX);
            writer.Write(SaveCoordZ);
            writer.Write(SaveRegionSizeXZ);
            writer.Write(SaveRegionSizePY);
            writer.Write(SaveRegionSizeNY);
        }

        public static FileHeader Read(BinaryReader reader) => new()
        {
            Version          = reader.ReadUInt32(),
            SaveCoordX       = reader.ReadInt32 (),
            SaveCoordZ       = reader.ReadInt32 (),
            SaveRegionSizeXZ = reader.ReadUInt32(),
            SaveRegionSizePY = reader.ReadInt32 (),
            SaveRegionSizeNY = reader.ReadInt32 (),
        };

        public readonly bool CompareIsValid(SaveCoord coord)
        {
            return 
                Version          == VERSION &&
                SaveCoordX       == coord.X &&
                SaveCoordZ       == coord.Z &&
                SaveRegionSizeXZ == SaveCoord.REGION_SIZE_XZ &&
                SaveRegionSizePY == WorldConstants.REGION_POS_CHUNKS &&
                SaveRegionSizeNY == WorldConstants.REGION_NEG_CHUNKS;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ChunkTable
    {
        public required long Location;
        public required Stream Stream;

        public static long Size => Marshal.SizeOf<TableRow>() * SaveCoord.REGION_AREA * WorldConstants.REGION_TOTAL_CHUNKS;

        public readonly TableRow[] ReadAllRows(BinaryReader reader)
        {
            Stream.Position = Location;

            TableRow[] rows = new TableRow[SaveCoord.REGION_AREA * WorldConstants.REGION_TOTAL_CHUNKS];

            int i = 0;
            for (int z = 0; z < SaveCoord.REGION_SIZE_XZ; z++)
            {
                for (int x = 0; x < SaveCoord.REGION_SIZE_XZ; x++)
                {
                    for (int y = 0; y < WorldConstants.REGION_TOTAL_CHUNKS; y++, i++)
                    {
                        rows[i] = TableRow.Read(reader);
                    }
                }
            }

            return rows;
        }

        public readonly void WriteRow(BinaryWriter writer, SaveCoord sCoord, ChunkCoord cCoord, TableRow row)
        {
            int xIDx = SaveCoord.Convert_Region_Coord_To_Offset(cCoord.X);
            int yIdx = cCoord.Y + WorldConstants.REGION_NEG_CHUNKS;
            int zIDx = SaveCoord.Convert_Region_Coord_To_Offset(cCoord.Z);
            int rowIdx = 
                yIdx + 
                xIDx * SaveCoord.REGION_SIZE_XZ + 
                zIDx * SaveCoord.REGION_AREA;

            Stream.Position = Location + Marshal.SizeOf<TableRow>() * rowIdx;

            row.Write(writer);
        }

        public long FindFreeSpace(IEnumerable<TableRow> table, long requiredLength)
        {
            long tableEndOffset = Location + Size;

            // Step 1: Sort the table rows by their Location (ascending)
            var sortedTable = table.OrderBy(row => row.Location).ToList();

            // Step 2: Check between the table end and the first chunk
            if (sortedTable.Count > 0)
            {
                long firstChunkStart = sortedTable[0].Location;
                if (firstChunkStart - tableEndOffset >= requiredLength)
                    return tableEndOffset;
            }
            else
            {
                // Table is empty, place directly after the table
                return tableEndOffset;
            }

            // Step 3: Iterate and check gaps between existing chunks
            for (int i = 0; i < sortedTable.Count - 1; i++)
            {
                long currentChunkEnd = sortedTable[i].Location + sortedTable[i].Length;
                long nextChunkStart = sortedTable[i + 1].Location;

                long gap = nextChunkStart - currentChunkEnd;

                if (gap >= requiredLength)
                    return currentChunkEnd;
            }

            // Step 4: No suitable gap found, append at the end of the last chunk
            var lastChunk = sortedTable.Last();
            return lastChunk.Location + lastChunk.Length;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TableRow
        {
            public bool  IsValid;
            public int   XPos;
            public int   YPos;
            public int   ZPos;
            public int   Encoding;
            public long  Location;
            public long  Length;

            public static TableRow Read(BinaryReader reader) => new TableRow()
            {
                IsValid  = reader.ReadBoolean(),
                XPos     = reader.ReadInt32(),
                YPos     = reader.ReadInt32(),
                ZPos     = reader.ReadInt32(),
                Encoding = reader.ReadInt32(),
                Location = reader.ReadInt64(),
                Length   = reader.ReadInt64(),
            };

            public readonly void Write(BinaryWriter writer)
            {
                writer.Write(IsValid );
                writer.Write(XPos    );
                writer.Write(YPos    );
                writer.Write(ZPos    );
                writer.Write(Encoding);
                writer.Write(Location);
                writer.Write(Length  );
            }
        }
    }
}
