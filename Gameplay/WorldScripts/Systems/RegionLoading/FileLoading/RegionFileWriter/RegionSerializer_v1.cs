using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.ChunkSerializers;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.RegionSerialization
{
    public class RegionSerializer_v1 : IRegionSerializer
    {
        const long CHUNK_TABLE_DATA_LENGTH = 32;

        SerializerSelector selector = new();

        public void Serialize(FileStream stream, RegionCoord coord, RegionData data)
        {
            using BinaryWriter writer = new(stream, System.Text.Encoding.Default, leaveOpen: true);

            // Reset pointer
            stream.Position = 0;

            // Version ID
            writer.Write(1); // Version 1

            // Base coord for this region
            writer.Write(coord.X);
            writer.Write(coord.Z);

            // Number of chunks in the region
            writer.Write(data.Chunks.Length);

            // Pointer to the chunk table
            long tablePtr = stream.Position;
            // Skip to writing chunks
            stream.Position += CHUNK_TABLE_DATA_LENGTH * data.Chunks.Length;

            // Write the chunk table
            long chunkDataPtr = stream.Position;
            for (int i = 0; i < data.Chunks.Length; i++)
            {
                var chunk = data.Chunks[i];
                var serialized = selector.Serialize(chunk, out int algorithm);

                //--------------------
                // Chunk Header Table
                //--------------------

                // Move to the header table position
                stream.Position = tablePtr + CHUNK_TABLE_DATA_LENGTH * i;

                // Write the relative position
                writer.Write(0); // X Position
                writer.Write(i - REGION_Y_NEG_COUNT); // Y Position
                writer.Write(0); // Z Position

                // Write the compression algorithm
                writer.Write(algorithm);
                // 0 - Empty (Fully air - no data saved)
                // 1 - No compression

                // Pointer to the data
                writer.Write(chunkDataPtr);
                // Data length
                writer.Write(serialized.Length);

                //--------------------
                // Chunk Data
                //--------------------

                // Write the chunk's data
                stream.Position = chunkDataPtr;
                stream.Write(serialized);
                chunkDataPtr = stream.Position;
            }
        }

        public bool Deserialize(FileStream stream, out RegionCoord coord, out RegionData data)
        {
            using BinaryReader reader = new(stream, System.Text.Encoding.Default, leaveOpen: true);

            stream.Position = 0;

            coord = default;
            data = default!;

            // Wrong version for this decoder
            int version = reader.ReadInt32();
            if (version != 1)
                return false;

            // TODO Confirm coord correct
            coord = new RegionCoord(
                reader.ReadInt32(),
                reader.ReadInt32()
            );

            // Y Count incorrect for game
            int yChunkCount = reader.ReadInt32();
            if (yChunkCount != REGION_Y_TOTAL_COUNT)
                return false;

            // Chunk Header Table
            long headerTablePtr = stream.Position;

            ChunkData[] chunkDatas = new ChunkData[yChunkCount];
            for (int i = 0; i < yChunkCount; i++)
            {
                stream.Position = headerTablePtr + i * CHUNK_TABLE_DATA_LENGTH;

                // Offset position
                var chunkCoord = new ChunkCoord(
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32()
                );

                // How was the data compressed
                int algorithm = reader.ReadInt32();

                // Pointer and length of chunk data
                long dataPtr = reader.ReadInt64();
                long length  = reader.ReadInt64();

                stream.Position = dataPtr;

                // Chunk is empty
                byte[] bytes = reader.ReadBytes((int)length);
                chunkDatas[i] = selector.Deserialize(algorithm, bytes);
            }

            data = new RegionData(chunkDatas);
            return true;
        }
    }
}
