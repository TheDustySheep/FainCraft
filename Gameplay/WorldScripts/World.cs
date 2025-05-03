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

namespace FainCraft.Gameplay.WorldScripts;
internal class World : GameObject
{
    public readonly IWorldData WorldData;
    public readonly IWorldEntityController WorldEntityController;

    readonly IRenderSystem            _renderSystem;
    readonly IMeshGenerationSystem    _meshSystem;
    readonly ITerrainGenerationSystem _terrainSystem;
    readonly IRegionLoadingController       _loadingController;
    readonly IActiveRegionController  _activeRegionController;

    public World()
    {
        var voxel_atlas    = ResourceLoader.LoadTextureAtlas(@"C:\Users\Sean\source\repos\FainCraft\Resources\Textures\atlas.png", 16, mipMapMode: Texture.MipMapModes.Nearest);
        var voxel_shader   = ResourceLoader.LoadShader(@"C:\Users\Sean\source\repos\FainCraft\Resources\Shaders\Voxel_Shader\");
        var voxel_material = new VoxelMaterial(voxel_shader, voxel_atlas);

        var indexer = VoxelIndexer.Builder.FromFilePath();

        WorldEntityController = new WorldEntityController();
        WorldData = new WorldData(indexer);
        _renderSystem = new RenderSystem(voxel_material);
        _meshSystem = new ThreadedMeshGenerationSystem(WorldData, _renderSystem, () => new MeshGenerator_v2(indexer));
        //_meshSystem = new BasicMeshGenerationSystem(WorldData, _renderSystem, new MeshGenerator_Bitwise(_indexer));
        _terrainSystem = new ThreadedTerrainGenerationSystem(new OverworldGenerator(indexer));
        _loadingController = new RegionLoadingController(WorldData, _terrainSystem, new NullFileLoader());

        var activeRegionController = new ActiveRegionController_v2();
        activeRegionController.OnLoad += _loadingController.Load;
        _activeRegionController = activeRegionController;

    }

    public override void Update()
    {
        _activeRegionController.Tick();
        _loadingController.Tick();
        _meshSystem.Tick();
        _renderSystem.Draw();
    }
}
