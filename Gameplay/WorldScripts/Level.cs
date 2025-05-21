using FainCraft.Gameplay.WorldEntities;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using FainCraft.Gameplay.WorldScripts.Signals;
using FainCraft.Gameplay.WorldScripts.Storage;
using FainCraft.Gameplay.WorldScripts.Systems;
using FainCraft.Gameplay.WorldScripts.Systems.Activation;
using FainCraft.Gameplay.WorldScripts.Systems.Loading;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Overworld;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.Materials;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems;
using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Resources;
using System;

namespace FainCraft.Gameplay.WorldScripts;

public class Level : GameObject
{
    private readonly ServiceContainer _provider;
    public IServiceProvider Provider => _provider;

    private readonly IRenderSystem _renderSystem;
    private readonly IMeshGenerationSystem _meshSystem;
    private readonly IRegionActivator _activator;

    public Level()
    {
        var voxel_atlas      = ResourceLoader.LoadTextureAtlas(@"Resources\Textures\atlas.png", 16, mipMapMode: Texture.MipMapModes.Nearest);
        var voxel_material_o = new VoxelMaterial(ResourceLoader.LoadShader(@"Resources\Shaders\Voxels\Voxel_Shader_2\"), voxel_atlas);
        var voxel_material_t = new VoxelMaterialTransparent(ResourceLoader.LoadShader(@"Resources\Shaders\Voxels\Voxel_Shader_Transparent\"), voxel_atlas);

        _provider = new ServiceContainer();

        // Event Bus
        _provider.RegisterSingleton<IEventBus>(new EventBus());

        // Entities
        _provider.RegisterSingleton<IWorldEntityController>(new WorldEntityController());

        // Indexing
        _provider.RegisterSingleton<IVoxelIndexer>(VoxelTypeLoader.LoadIndexer());

        // Data Loading
        _provider.RegisterSingleton<ITerrainGenerator>(new OverworldGenerator(_provider));
        _provider.RegisterSingleton<ITerrainGenerationSystem>(new ThreadedTerrainGenerationSystem(_provider));
        _provider.RegisterSingleton<IRegionDataLoader>(new SingleplayerDataLoader(_provider));

        // Data Access
        _provider.RegisterSingleton<IRegionDataStore>(new RegionDataStore(_provider));
        _provider.RegisterSingleton<IChunkDataStore>(new ChunkDataStore(_provider));
        _provider.RegisterSingleton<IChunkClusterDataStore>(new ChunkClusterDataStore(_provider));
        _provider.RegisterSingleton<IVoxelDataStore>(new VoxelDataStore(_provider));

        // Region Activation
        _activator = _provider.RegisterSingleton<IRegionActivator>(new RegionActivator(_provider));

        // Rendering
        _provider.RegisterFactory<IMeshGenerator>(() => new MeshGenerator_v3(_provider));
        _renderSystem = _provider.RegisterSingleton<IRenderSystem>(new RenderSystem(voxel_material_o, voxel_material_t, _provider));
        _meshSystem   = _provider.RegisterSingleton<IMeshGenerationSystem>(new MeshGenerationSystem(_provider, new ActiveRegionRadius(() => SharedVariables.RenderSettings.Value.RenderRadius)));
    }

    public override void Update()
    {
        _activator.Tick();
        _meshSystem.Tick();
        _renderSystem.Draw();
    }
}
