using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.RegionSerialization;
using System.Text;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.RegionFileWriter
{
    public class RegionSerializer_v3 : IRegionSerializer
    {
        public LoadResult Load(LoadRequest request)
        {

        }

        public SaveResult Save(SaveRequest request)
        {
            using var writer = new BinaryWriter(request.Stream, Encoding.Default, leaveOpen: true);
            using var reader = new BinaryReader(request.Stream, Encoding.Default, leaveOpen: true);

            FileHeader header = new()
            {
                VERSION        = 3,
                SaveCoordX     = request.SaveCoord.X,
                SaveCoordZ     = request.SaveCoord.Z,
                SaveRegionSize = SaveCoord.REGION_SIZE
            };
            header.Write(writer);
        }

        private struct FileHeader
        {
            public required uint VERSION;
            public required  int SaveCoordX;
            public required  int SaveCoordZ;
            public required uint SaveRegionSize;

            public readonly void Write(BinaryWriter writer)
            {
                writer.Write(VERSION);
                writer.Write(SaveCoordX);
                writer.Write(SaveCoordZ);
                writer.Write(SaveRegionSize);
            }

            public static FileHeader Read(BinaryReader reader) => new()
            {
                VERSION        = reader.ReadUInt32(),
                SaveCoordX     = reader.ReadInt32 (),
                SaveCoordZ     = reader.ReadInt32 (),
                SaveRegionSize = reader.ReadUInt32(),
            };
        }

        private struct ChunkTable
        {
            public readonly void Write(BinaryWriter writer)
            {

            }
        }
    }
}
