using FainCraft.Gameplay.WorldScripts.Chunking;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Voxels;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Physics;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts;
internal class PlayerMotor
{
    public Transform Transform;


    public Vector3 PlayerSize { get; set; } = new Vector3(0.8f, 1.8f, 0.8f);
    public Vector3 Velocity { get; set; }

    private Vector3? _movePositionRequest;
    private CollisionResult _groundProbe = new();

    public IWorldData WorldData;

    public PlayerMotor(Transform transform, IWorldData worldData)
    {
        WorldData = worldData;
        Transform = transform;
    }

    public void SetPosition(Vector3 request)
    {
        _movePositionRequest = request;
    }

    public void FixedUpdate()
    {
        // Phase 1
        HandleMovePosition();
        HandleInitialCollision();
        ProbeGround();

        // Phase 2
        SolveVelocity();
    }

    private void HandleMovePosition()
    {
        if (_movePositionRequest is null)
            return;

        Transform.Position = _movePositionRequest.Value;
        _movePositionRequest = null;
    }

    private void HandleInitialCollision()
    {

    }

    private void ProbeGround()
    {
        AABB playerAABB = new AABB()
        {
            Center = Transform.Position + new Vector3(0f, PlayerSize.Y, 0f),
            Size = PlayerSize,
        };

        var playerGlobalCoord = new GlobalVoxelCoord(Transform.Position);

        var indexer = WorldData.Indexer;
        _groundProbe.Indexer = indexer;

        int i = 0;
        for (int y = 0; y < 4; y++)
        {
            for (int z = 0; z < 3; z++)
            {
                for (int x = 0; x < 3; x++, i++)
                {
                    var offsetCoord = playerGlobalCoord + new GlobalVoxelCoord(-1, -1, -1);
                    WorldData.GetVoxelData(offsetCoord, out var voxelData);
                    _groundProbe.VoxelData[i] = voxelData;
                    _groundProbe.VoxelType[i] = indexer.GetVoxelType(voxelData.Index);
                }
            }
        }

        var groundType = _groundProbe.VoxelType[4];

        if (groundType is null)
            _groundProbe.IsGrounded = false;
        else
            _groundProbe.IsGrounded = groundType.Physics_Solid;

        Console.WriteLine($"Voxel at player is grounded: {_groundProbe.IsGrounded} Location: {playerGlobalCoord} {Transform.Position}");
    }

    private void SolveVelocity()
    {

    }

    private class CollisionResult
    {
        public bool IsGrounded;
        public Vector3 Normal = Vector3.UnitY;
        public VoxelIndexer? Indexer;
        public VoxelData[] VoxelData = new VoxelData[36];
        public VoxelType?[] VoxelType = new VoxelType?[36];
    }
}
