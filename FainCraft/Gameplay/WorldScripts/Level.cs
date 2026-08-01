using FainCraft.Gameplay.WorldEntities;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using FainCraft.Gameplay.WorldScripts.Signals;
using FainCraft.Gameplay.WorldScripts.Storage;
using FainCraft.Gameplay.WorldScripts.Systems;
using FainCraft.Gameplay.WorldScripts.Systems.Loading;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Generator_v2;
using FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement;
using FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement.Transitions;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.Materials;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGenerationSystems;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems;
using FainEngine_v2.Core;
using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Resources;
using FainEngine_v2.Utils;

namespace FainCraft.Gameplay.WorldScripts;

public class Level : GameObject
{
    private readonly RenderingSystem _renderController;

    public readonly ISignalBus SignalBus;
    public readonly IWorldEntityController EntityController;
    public readonly IVoxelIndexer VoxelIndexer;
    public readonly IRegionDataStore RegionStore;
    public readonly IChunkDataStore ChunkStore;
    public readonly IChunkClusterDataStore ChunkClusterStore;
    public readonly IVoxelDataStore VoxelStore;

    private readonly RegionStateManager _regionStateManager;

    public Level()
    {
        var voxel_atlas      = ResourceLoader.LoadTextureAtlas(@"Resources\Textures\atlas.png", 16, mipMapMode: Texture.MipMapModes.Nearest);
        var voxel_material_o = new VoxelMaterial(ResourceLoader.LoadShader(@"Resources\Shaders\Voxels\Voxel_Shader_2\"), voxel_atlas);
        var voxel_material_t = new VoxelMaterialTransparent(ResourceLoader.LoadShader(@"Resources\Shaders\Voxels\Voxel_Shader_Transparent\"), voxel_atlas);

        // Event Bus
        SignalBus = new EventBus();

        // Entities
        EntityController = new WorldEntityController();

        // Indexing
        VoxelIndexer = VoxelTypeLoader.LoadIndexer();

        // Data Loading
        var terrainGenerator = new ThreadedTerrainGenerationSystem(new Overworld_v2(VoxelIndexer));
        var dataLoader = new SingleplayerDataLoader(terrainGenerator);

        // Data Access
        RegionStore       = new RegionDataStore(SignalBus);
        ChunkStore        = new ChunkDataStore(SignalBus, RegionStore);
        ChunkClusterStore = new ChunkClusterDataStore(ChunkStore);
        VoxelStore        = new VoxelDataStore(SignalBus, ChunkStore);

        // Rendering
        _renderController = new RenderingSystem(
            SignalBus,
            new MeshGenerationSystem(
                () => SharedVariables.PlayerPosition.Value.RegionCoord,
                ChunkStore,
                ChunkClusterStore,
                () => new MeshGenerator_v3(VoxelIndexer)
            ),
            new DrawingSystem(voxel_material_o, voxel_material_t, VoxelIndexer)
        );

        // Region loading
        var transitionController = new TransitionController();

        // Load Data
        transitionController.Register(StateType.Unloaded, StateType.Loaded, new TransitionFactory(coord => new TransitionLoadData(dataLoader, RegionStore, coord)));
        transitionController.Register(StateType.Loaded, StateType.Unloaded, new TransitionFactory(coord => new TransitionUnloadData(dataLoader, RegionStore, coord)));

        // Load Meshes
        transitionController.Register(StateType.Loaded, StateType.Meshed, new TransitionFactory(coord => new TransitionLoadMesh(_renderController, coord)));
        transitionController.Register(StateType.Meshed, StateType.Loaded, new TransitionFactory(coord => new TransitionUnloadMesh(_renderController, coord)));

        // Inject into the state manager
        _regionStateManager = new RegionStateManager(
            new ActiveRegionRadius(() => SharedVariables.RenderSettings.Value.LoadRadius),
            transitionController
        );
    }

    public override void Update()
    {
        // Force refresh of region generation
        if (DependencyInjector.Resolve<IGameInputs>().IsKeyDown(Silk.NET.Input.Key.U))
        {
            var coord = SharedVariables.PlayerPosition.Value.RegionCoord;

            if (RegionStore.GetRegion(coord, out var data))
            {
                SignalBus.Publish(new ModifiedRegionDataSignal()
                {
                    Coord = coord,
                    Data = data
                });
            }
        }

        _regionStateManager.Tick();
        _renderController.Tick();
    }
}
