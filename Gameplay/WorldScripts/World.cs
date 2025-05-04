using FainCraft.Gameplay.WorldEntities;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.ActiveRegionControl;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld;
using FainCraft.Gameplay.WorldScripts.Voxels;
using FainCraft.Resources.Shaders.Voxel_Shader;
using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Resources;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.RegionSerialization;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.ChunkSerializers;
using FainCraft.Gameplay.WorldScripts.Core;

namespace FainCraft.Gameplay.WorldScripts;
internal class World : GameObject
{
    public readonly IWorldData WorldData;
    public readonly IWorldEntityController WorldEntityController;

    readonly IRenderSystem            _renderSystem;
    readonly IMeshGenerationSystem    _meshSystem;
    readonly ITerrainGenerationSystem _terrainSystem;
    readonly IRegionLoadingController _loadingController;
    readonly IActiveRegionController  _activeRegionController;

    readonly IFileLoadingSystem       _fileLoadingSystem;

    public World()
    {
        var voxel_atlas    = ResourceLoader.LoadTextureAtlas(@"C:\Users\Sean\source\repos\FainCraft\Resources\Textures\atlas.png", 16, mipMapMode: Texture.MipMapModes.Nearest);
        var voxel_shader   = ResourceLoader.LoadShader(@"C:\Users\Sean\source\repos\FainCraft\Resources\Shaders\Voxel_Shader\");
        var voxel_material = new VoxelMaterial(voxel_shader, voxel_atlas);

        var indexer = VoxelIndexer.Builder.FromFilePath();

        // Entities
        WorldEntityController = new WorldEntityController();

        // Data
        WorldData = new WorldData(indexer);

        // Rendering
        _renderSystem = new RenderSystem(voxel_material);
        _meshSystem   = new ThreadedMeshGenerationSystem(WorldData, _renderSystem, () => new MeshGenerator_v2(indexer));

        // Terrain / Loading
        _fileLoadingSystem = new BasicFileLoader("Save_1", new RegionSerializer_v1(new NoCompressionSerializer()));

        WorldData.OnChunkUpdate += SavingFunc;

        _terrainSystem     = new ThreadedTerrainGenerationSystem(new OverworldGenerator(indexer));
        _loadingController = new RegionLoadingController(WorldData, _terrainSystem, _fileLoadingSystem);


        // Active region control
        var activeRegionController = new ActiveRegionController_v2();
        activeRegionController.OnLoad   += _loadingController.Load;
        activeRegionController.OnUnload += _loadingController.Unload;
        _activeRegionController = activeRegionController;

    }

    private void SavingFunc(ChunkCoord coord, bool important)
    {
        if (!important)
            return;

        var regionCoord = (RegionCoord)coord;
        var data = WorldData.GetRegion(regionCoord);
        if (data is null)
            return;
        _fileLoadingSystem.Save(regionCoord, data);
    }

    public override void Update()
    {
        _activeRegionController.Tick();
        _loadingController.Tick();
        _meshSystem.Tick();
        _renderSystem.Draw();
    }
}
