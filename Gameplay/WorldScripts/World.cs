using FainCraft.Gameplay.WorldEntities;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.ActiveRegionControl;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld;
using FainCraft.Gameplay.WorldScripts.Voxels;
using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Resources;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.RegionSerialization;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.Materials;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting;

namespace FainCraft.Gameplay.WorldScripts;
internal class World : GameObject
{
    public readonly IWorldData WorldData;
    public readonly IWorldEntityController WorldEntityController;

    readonly IRenderSystem            _renderSystem;
    readonly ILightingSystem          _lightingSystem;
    readonly IMeshGenerationSystem    _meshSystem;
    readonly ITerrainGenerationSystem _terrainSystem;
    readonly IRegionLoadingController _loadingController;
    readonly IActiveRegionController  _activeRegionController;

    readonly IFileLoadingSystem       _fileLoadingSystem;

    public World()
    {
        var voxel_atlas      = ResourceLoader.LoadTextureAtlas(@"Resources\Textures\atlas.png", 16, mipMapMode: Texture.MipMapModes.Nearest);
        var voxel_material_o = new VoxelMaterial           (ResourceLoader.LoadShader(@"Resources\Shaders\Voxels\Voxel_Shader_2\"), voxel_atlas);
        var voxel_material_t = new VoxelMaterialTransparent(ResourceLoader.LoadShader(@"Resources\Shaders\Voxels\Voxel_Shader_Transparent\"), voxel_atlas);

        var indexer = VoxelIndexer.Builder.FromFilePath();

        // Entities
        WorldEntityController = new WorldEntityController();

        // Data
        WorldData = new WorldData(indexer);

        // Rendering
        _renderSystem   = new RenderSystem(voxel_material_o, voxel_material_t);
        _meshSystem     = new ThreadedMeshGenerationSystem(WorldData, _renderSystem, () => new MeshGenerator_v3(indexer));
        _lightingSystem = new LightingSystem(WorldData, _renderSystem, new LightingCalculator(indexer));

        // Terrain / Loading


        _terrainSystem     = new ThreadedTerrainGenerationSystem(new OverworldGenerator(indexer));

        bool enableSaving = false;

        if (enableSaving)
            _fileLoadingSystem = new BasicFileLoader("Save_1", new RegionSerializer_v1());
        else
            _fileLoadingSystem = new NullFileLoader();

        WorldData.OnChunkUpdate += SavingFunc;
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

        if (!WorldData.GetRegion(regionCoord, out var data))
            return;

        _fileLoadingSystem.Save(regionCoord, data);
    }

    public override void Update()
    {
        _activeRegionController.Tick();
        _loadingController.Tick();
        _meshSystem.Tick();
        _lightingSystem.Tick();
        _renderSystem.Draw();
    }
}
