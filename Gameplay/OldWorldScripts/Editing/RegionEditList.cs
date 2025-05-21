using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;
using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraft.Gameplay.OldWorldScripts.Editing
{
    public class RegionEditList
    {
        private readonly Dictionary<RegionCoord, List<RegionEdit>> EditList = [];

        public void AddEdit(VoxelCoordGlobal coord, IVoxelEdit edit)
        {
            var region = (RegionCoord)coord;

            var pair = new RegionEdit()
            {
                Coord = new VoxelCoordRegion(coord),
                Edit = edit
            };

            if (EditList.TryGetValue(region, out var list))
                list.Add(pair);
            else
                EditList[region] = [pair];
        }

        public void AddMany(RegionCoord regionCoord, IEnumerable<RegionEdit> edits)
        {
            if (EditList.TryGetValue(regionCoord, out var list))
                list.AddRange(edits);
            else
                EditList[regionCoord] = edits.ToList();
        }

        public void Combine(RegionEditList other)
        {
            foreach (var pair in other.EditList)
            {
                AddMany(pair.Key, pair.Value);
            }
        }

        public void ApplyEdits(RegionCoord regionCoord, RegionData data, List<ChunkCoord>? remeshChunks = null)
        {
            if (!EditList.Remove(regionCoord, out var list))
                return;

            foreach (var pair in list)
            {
                var coord = pair.Coord;
                
                if (!data.GetChunk(coord.Chunk_Y, out var chunk))
                    continue;

                // UpdateMesh the voxel
                int index = coord.LocalCoord.VoxelIndex;
                var _oldState = chunk.VoxelData[index];
                var _newState = pair.Edit.Execute(_oldState);
                chunk.VoxelData[index] = _newState;

                if (remeshChunks != null && _oldState != _newState)
                {
                    var localCoord = coord.LocalCoord;
                    var chunkCoord = (ChunkCoord)regionCoord;

                    if (localCoord.X == 0)
                        remeshChunks.Add(chunkCoord + new ChunkCoord(-1, 0, 0));
                    else if (localCoord.X == CHUNK_SIZE - 1)
                        remeshChunks.Add(chunkCoord + new ChunkCoord(1, 0, 0));

                    if (localCoord.Y == 0)
                        remeshChunks.Add(chunkCoord + new ChunkCoord(0, -1, 0));
                    else if (localCoord.Y == CHUNK_SIZE - 1)
                        remeshChunks.Add(chunkCoord + new ChunkCoord(0, 1, 0));

                    if (localCoord.Z == 0)
                        remeshChunks.Add(chunkCoord + new ChunkCoord(0, 0, -1));
                    else if (localCoord.Z == CHUNK_SIZE - 1)
                        remeshChunks.Add(chunkCoord + new ChunkCoord(0, 0, 1));
                }
            }
        }
    }
}
