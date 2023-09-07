using FainCraft.Gameplay.WorldScripts.Chunking;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Voxels;
using FainEngine_v2.Core;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Physics;
using FainEngine_v2.Utils;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts;
internal class PlayerMotor
{
    #region References
    public Transform Transform;
    public IWorldData WorldData;
    #endregion

    #region Public fields
    public float Gravity { get; set; } = 30f;
    public Vector3 PlayerSize { get; set; } = new Vector3(0.4f, 1.8f, 0.4f);
    public Vector3 Velocity { get; set; }
    #endregion

    #region Private fields
    private readonly int _physicsIterations = 8;
    private Vector3 _velocityAdd;
    private Vector3? _movePositionRequest;
    private readonly CollisionResult _groundProbe = new();
    #endregion


    public PlayerMotor(Transform transform, IWorldData worldData)
    {
        WorldData = worldData;
        Transform = transform;
    }

    public void AddVelocity(Vector3 velocity)
    {
        _velocityAdd = velocity;
    }

    public void SetPosition(Vector3 request)
    {
        _movePositionRequest = request;
    }

    public void FixedUpdate()
    {
        // Phase 1 - Initial conditions
        HandleMovePosition();
        //ResolveInitialOverlap();

        // Phase 2 - Resolve collisions
        ProbeGround();
        SolveVelocity();
        ResolveVoxelCollision();
    }

    private void HandleMovePosition()
    {
        if (_movePositionRequest is null)
            return;

        Transform.Position = _movePositionRequest.Value;
        _movePositionRequest = null;
    }

    private void UpdateVoxelsAround(GlobalVoxelCoord playerVoxel)
    {
        var indexer = WorldData.Indexer;
        _groundProbe.Indexer = indexer;

        for (int y = 0; y < 2; y++)
        {
            for (int z = 0; z < 3; z++)
            {
                for (int x = 0; x < 3; x++)
                {
                    WorldData.GetVoxelData(playerVoxel + new GlobalVoxelCoord(-1, 0, -1), out var voxelData);
                    _groundProbe.SolidVoxels[x, y, z] = indexer.GetVoxelType(voxelData.Index).Physics_Solid;
                }
            }
        }
    }

    private void ResolveInitialOverlap()
    {
        DynamicAABB playerAABB = new()
        {
            Position = Transform.Position - new Vector3(PlayerSize.X / 2, 0f, PlayerSize.Z / 2),
            Size = PlayerSize,
            Velocity = Velocity
            //Velocity = Vector3.UnitY * -GameTime.FixedUpdate * 9.81f,
        };

        var playerGlobalCoord = new GlobalVoxelCoord(Transform.Position);
        var offsetCoord = playerGlobalCoord + new GlobalVoxelCoord(-1, -1, -1);

        float totalDeltaTime = GameTime.FixedUpdate;

        for (int z = 0; z < 3; z++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (!_groundProbe.SolidVoxels[x, 0, z])
                    continue;
            }
        }

        playerAABB.Position += playerAABB.Velocity * totalDeltaTime;

        Transform.Position =
            playerAABB.Position +
            new Vector3(PlayerSize.X / 2, 0f, PlayerSize.Z / 2);
    }

    private void DoPhysicsTimeStep(ref DynamicAABB playerAABB, GlobalVoxelCoord playerVoxel)
    {
        for (int y = 0; y < 2; y++)
        {
            for (int z = 0; z < 3; z++)
            {
                for (int x = 0; x < 3; x++)
                {
                    if (!_groundProbe.SolidVoxels[x, y, z])
                        continue;

                    StaticAABB voxelAABB = new()
                    {
                        Size = Vector3.One,
                        Position = new Vector3(x, y, z) + (Vector3)playerVoxel - new Vector3(1, 0, 1),
                    };

                    // Find all the colliders that will hit
                    AABBSolver.Collide(ref playerAABB, voxelAABB, AABBSolver.CollisionMode.Slide);
                }
            }
        }

        playerAABB.Position += playerAABB.Velocity;
    }

    private void ResolveVoxelCollision()
    {
        DynamicAABB playerAABB = new()
        {
            Position = Transform.Position - new Vector3(PlayerSize.X / 2, 0f, PlayerSize.Z / 2),
            Size = PlayerSize,
            Velocity = Velocity * GameTime.FixedUpdate / _physicsIterations
        };

        GlobalVoxelCoord playerVoxel = new(Transform.Position);

        UpdateVoxelsAround(playerVoxel);

        for (int i = 0; i < _physicsIterations; i++)
            DoPhysicsTimeStep(ref playerAABB, playerVoxel);

        //totalDeltaTime = PhysicsAABB.Collide(ref playerAABB, voxelAABB, totalDeltaTime, PhysicsAABB.CollisionMode.Slide);
        // Add any remaining velocity
        //playerAABB.Position += playerAABB.Velocity * totalDeltaTime;

        Transform.Position =
            playerAABB.Position +
            new Vector3(PlayerSize.X / 2, 0f, PlayerSize.Z / 2);
    }


    private void ProbeGround()
    {
        _groundProbe.IsGrounded = _groundProbe.SolidVoxels[1, 0, 1];
    }

    private void SolveVelocity()
    {
        Vector3 velocity = Velocity;

        velocity += _velocityAdd;
        _velocityAdd = Vector3.Zero;

        //if (!_groundProbe.IsGrounded)
        //    velocity.Y = MathUtils.MoveTowards(velocity.Y, -Gravity, GameTime.FixedUpdate * Gravity);
        //else
        //    velocity.Y = MathF.Max(0f, velocity.Y);

        Velocity = velocity;
    }

    private class CollisionResult
    {
        public bool IsGrounded;
        public Vector3 Normal = Vector3.UnitY;
        public VoxelIndexer? Indexer;
        public bool[,,] SolidVoxels = new bool[3, 2, 3];
    }
}
