using FainCraft.Gameplay.WorldScripts.Systems.Loading;
using FainCraft.Gameplay.WorldScripts.Systems.MeshGeneration;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering;
using FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration;
using FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.BasicWorld;
using FainCraft.Gameplay.WorldScripts.Voxels;
using FainCraft.Resources.Shaders.Voxel_Shader;
using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Resources;

namespace FainCraft.Gameplay.WorldScripts.Chunking;
internal class World : GameObject, IEntity
{

    readonly IRenderSystem renderSystem;

    public IWorldData WorldData;

    readonly IMeshGenerationSystem meshSystem;
    readonly ITerrainGenerationSystem terrainSystem;
    readonly ILoadingController loadingController;
    readonly IActiveRegionController activeRegionController;

    public World()
    {
        var voxel_atlas = ResourceLoader.LoadTexture2D(@"C:\Users\Sean\source\repos\FainCraft\Resources\Textures\atlas.png", mipMapMode:Texture2D.MipMapModes.Nearest);
        var voxel_shader = ResourceLoader.LoadShader(@"C:\Users\Sean\source\repos\FainCraft\Resources\Shaders\Voxel_Shader\");
        var voxel_material = new VoxelMaterial(voxel_shader, voxel_atlas);

        var indexer = new VoxelIndexer();
        indexer.LoadVoxels();

        WorldData = new WorldData(indexer);
        renderSystem = new RenderSystem(voxel_material);
        meshSystem = new ThreadedMeshGenerationSystem(WorldData, renderSystem, new MeshGenerator(indexer));
        terrainSystem = new ThreadedTerrainGenerationSystem(WorldData, new OverworldGenerator(indexer));
        loadingController = new LoadingController(WorldData, terrainSystem);
        activeRegionController = new ActiveRegionController(loadingController);
    }

    public void Update()
    {
        activeRegionController.Tick();
        loadingController.Tick();
        terrainSystem.Tick();
        meshSystem.Tick();
        renderSystem.Draw();
    }
}
