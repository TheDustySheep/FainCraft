using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Voxels;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting;

public class LightingCalculator : ILightingCalculator
{
    private readonly VoxelIndexer _indexer;
    private const byte MAX_LIGHT = 15;

    private const int CHUNK_SIZE = 32;
    private const int REGION_CHUNKS_Y = 6;
    private const int REGION_HEIGHT = CHUNK_SIZE * REGION_CHUNKS_Y;
    private const int PAD_XZ = 15;
    private const int PAD_Y = 1;

    private static readonly (int dx, int dy, int dz)[] Directions = new[]
    {
        ( 1,  0,  0), (-1,  0,  0),
        ( 0,  1,  0), ( 0, -1,  0),
        ( 0,  0,  1), ( 0,  0, -1)
    };

    public LightingCalculator(VoxelIndexer indexer)
    {
        _indexer = indexer;
    }

    public void Calculate(LightingRegionData data)
    {
        var lightPass = _indexer.CacheLightPassThrough.Data;
        var emitsLight = _indexer.CacheEmitsLight.Data;

        var skyQueue   = data.SkyQueue;
        var torchQueue = data.TorchQueue;

        // INITIALIZE TORCH EMITTERS
        for (int y = -PAD_Y; y <= REGION_HEIGHT; y++)
            for (int z = -PAD_XZ; z < CHUNK_SIZE + PAD_XZ; z++)
                for (int x = -PAD_XZ; x < CHUNK_SIZE + PAD_XZ; x++)
                {
                    if (!data.GetVoxel(x, y, z, out var st)) continue;
                    byte emit = emitsLight[st.Index];
                    var cell = data[x, y, z];
                    cell.Torch = emit;
                    cell.Sky = 0;
                    data[x, y, z] = cell;
                    if (emit > 0) torchQueue.Enqueue((x, y, z));
                }

        // INITIALIZE SKY: always set top layer to MAX_LIGHT, then down until blocked
        for (int z = -PAD_XZ; z < CHUNK_SIZE + PAD_XZ; z++)
            for (int x = -PAD_XZ; x < CHUNK_SIZE + PAD_XZ; x++)
            {
                // Topmost voxel always gets full sky light
                if (data.GetVoxel(x, REGION_HEIGHT, z, out var topState))
                {
                    var topCell = data[x, REGION_HEIGHT, z];
                    topCell.Sky = MAX_LIGHT;
                    data[x, REGION_HEIGHT, z] = topCell;
                    skyQueue.Enqueue((x, REGION_HEIGHT, z));
                }

                // Propagate downward, attenuating only when blocked
                for (int y = REGION_HEIGHT - 1; y >= -PAD_Y; y--)
                {
                    if (!data.GetVoxel(x, y, z, out var state)) break;
                    if (!lightPass[state.Index]) break;
                    var cell = data[x, y, z];
                    cell.Sky = MAX_LIGHT;
                    data[x, y, z] = cell;
                    skyQueue.Enqueue((x, y, z));
                }
            }

        // PROPAGATE TORCH LIGHT
        while (torchQueue.Count > 0)
        {
            var (cx, cy, cz) = torchQueue.Dequeue();
            var curr = data[cx, cy, cz].Torch;
            if (curr <= 1) continue;
            byte next = (byte)(curr - 1);
            foreach (var (dx, dy, dz) in Directions)
            {
                int nx = cx + dx, ny = cy + dy, nz = cz + dz;
                if (!data.GetVoxel(nx, ny, nz, out var st)) continue;
                if (!lightPass[st.Index]) continue;
                var nCell = data[nx, ny, nz];
                if (nCell.Torch < next)
                {
                    nCell.Torch = next;
                    data[nx, ny, nz] = nCell;
                    torchQueue.Enqueue((nx, ny, nz));
                }
            }
        }

        // PROPAGATE SKY LIGHT
        while (skyQueue.Count > 0)
        {
            var (cx, cy, cz) = skyQueue.Dequeue();
            var curr = data[cx, cy, cz].Sky;
            foreach (var (dx, dy, dz) in Directions)
            {
                int nx = cx + dx, ny = cy + dy, nz = cz + dz;
                if (!data.GetVoxel(nx, ny, nz, out var st)) continue;
                if (!lightPass[st.Index]) continue;

                // no attenuation downwards
                byte next = (dy == -1) ? curr : (byte)(curr > 0 ? curr - 1 : 0);
                var nCell = data[nx, ny, nz];
                if (nCell.Sky < next)
                {
                    nCell.Sky = next;
                    data[nx, ny, nz] = nCell;
                    skyQueue.Enqueue((nx, ny, nz));
                }
            }
        }

        // FINAL PASS: for solid voxels within SSBO bounds, set their light to max(neighbors) - 1
        for (int y = -1; y < REGION_HEIGHT + 1; y++)
        {
            for (int z = -1; z < CHUNK_SIZE + 1; z++)
            {
                for (int x = -1; x < CHUNK_SIZE + 1; x++)
                {
                    if (!data.GetVoxel(x, y, z, out var st)) continue;
                    if (lightPass[st.Index]) continue; // only solid blocks

                    byte maxTorch = 0;
                    byte maxSky = 0;
                    foreach (var (dx, dy, dz) in Directions)
                    {
                        int nx = x + dx, ny = y + dy, nz = z + dz;
                        if (!data.GetVoxel(nx, ny, nz, out var nSt)) continue;
                        var neighbor = data[nx, ny, nz];
                        maxTorch = Math.Max(maxTorch, (byte)(neighbor.Torch > 0 ? neighbor.Torch - 1 : 0));
                        maxSky = Math.Max(maxSky, (byte)(neighbor.Sky > 0 ? neighbor.Sky - 1 : 0));
                    }
                    var cell = data[x, y, z];
                    cell.Torch = maxTorch;
                    cell.Sky = maxSky;
                    data[x, y, z] = cell;
                }
            }
        }
    }
}