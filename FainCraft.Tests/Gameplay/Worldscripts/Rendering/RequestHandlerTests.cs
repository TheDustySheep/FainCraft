using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Signals;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGenerationSystems;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Tests.Gameplay.Worldscripts.Rendering;

public class RequestHandlerTests
{
    private readonly Mock<ISignalBus> _mockSignalBus;
    private readonly List<(ChunkCoord, bool)> _updatedChunks;
    private readonly RequestHandler _handler;

    public RequestHandlerTests()
    {
        _mockSignalBus = new Mock<ISignalBus>();
        _updatedChunks = new List<(ChunkCoord, bool)>();
        //_handler = new RequestHandler(
        //    _mockSignalBus.Object,
        //    (coord, update) => _updatedChunks.AddClass((coord, update))
        //);
    }

    [Fact]
    public void OnLoadedRegionData_ShouldUpdateSurroundingChunks()
    {
        var signal = new LoadedRegionDataSignal
        {
            Coord = new RegionCoord(0, 0)
        };

        _handler.GetType()
                .GetMethod("OnLoadedRegionData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_handler, new object[] { signal });

        Assert.Contains(_updatedChunks, entry => ((RegionCoord)entry.Item1) == new RegionCoord(0, 0) && entry.Item2 == true);
    }

    [Fact]
    public void OnModifiedRegionData_ShouldUpdateCurrentAndNeighborChunks()
    {
        var signal = new ModifiedRegionDataSignal
        {
            Coord = new RegionCoord(1, 1)
        };

        _handler.GetType()
                .GetMethod("OnModifiedRegionData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_handler, new object[] { signal });

        Assert.True(_updatedChunks.Count > 0);
    }

    [Fact]
    public void OnModifiedChunkData_ShouldUpdateChunkAndNeighbors()
    {
        var coord = new ChunkCoord(new RegionCoord(0, 0), 0);
        var signal = new ModifiedChunkDataSignal { Coord = coord };

        _handler.GetType()
                .GetMethod("OnModifiedChunkData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_handler, new object[] { signal });

        Assert.Contains(_updatedChunks, entry => entry.Item1.Equals(coord));
    }

    [Fact]
    public void OnModifiedVoxelState_ShouldUpdateChunkAndTouchedNeighbors()
    {
        var coord = new VoxelCoordGlobal(0, 0, 0);
        var signal = new ModifiedVoxelStateSignal { Coord = coord };

        _handler.GetType()
                .GetMethod("OnModifiedVoxelState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_handler, new object[] { signal });

        Assert.True(_updatedChunks.Count > 0);
    }
}
