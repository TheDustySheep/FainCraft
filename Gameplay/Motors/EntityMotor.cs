using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using FainEngine_v2.Core;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Extensions;
using FainEngine_v2.Physics.AABB;
using FainEngine_v2.Utils;
using System.Numerics;

namespace FainCraft.Gameplay.Motors;
public class EntityMotor
{
    #region Public Properties
    public bool EnableGravity = true;
    public bool EnableCollision = true;
    public GroundedState GroundedState => _groundedState;

    public float Gravity
    {
        get => _gravity;
        set => _gravity = value;
    }
    public Vector3 PlayerSize
    {
        get => _playerSize;
        set => _playerSize = value;
    }

    public Vector3 TransformPosition
    {
        get => _transform.LocalPosition - (PlayerSize / 2);
        set => _transform.LocalPosition = (PlayerSize / 2) + value;
    }

    public Vector3 Velocity
    {
        get => _currentVelocity;
        set => _currentVelocity = value;
    }
    #endregion

    const float MAX_STABLE_MOVE_SPEED = CollisionHandler.IterationCount / GameTime.FixedDeltaTime * 0.5f;

    readonly Transform _transform;
    readonly CollisionHandler _collisionHandler;

    float _lastTime;
    float _gravity = 27f;

    public static readonly float DEFAULT_GRAVITY = 27f;

    GroundedState _groundedState;
    Vector3 _currentPosition;
    Vector3 _lastPosition;
    Vector3 _currentVelocity;
    Vector3 _playerSize = new Vector3(0.4f, 1.8f, 0.4f);

    public EntityMotor(CollisionHandler collisionHandler, Transform transform)
    {
        _transform = transform;
        _collisionHandler = collisionHandler;
        _currentPosition = TransformPosition;
        _lastPosition = TransformPosition;
        _lastTime = GameTime.TotalTime;
    }

    #region Public Functions

    public void Update()
    {
        TransformPosition = InterpolatePosition();
    }

    private Vector3 InterpolatePosition()
    {
        // One frame in the past
        float t = MathUtils.InvLerp(_lastTime, _lastTime + GameTime.FixedDeltaTime, GameTime.TotalTime);
        t = MathUtils.Clamp01(t);
        return Vector3.Lerp(_lastPosition, _currentPosition, t);
    }

    public void FixedUpdate()
    {
        // Initial collisions
        UpdateInitialInternals();

        // Collisions
        HandleCollisions();

        // Next velocity
        UpdateGrounded();
        UpdateVelocity();
    }
    #endregion

    private void UpdateInitialInternals()
    {
        _lastPosition = _currentPosition;
        _lastTime = GameTime.TotalTime;
    }

    public void HandleCollisions()
    {
        DynamicAABB playerAABB = new DynamicAABB()
        {
            Position = _currentPosition,
            Size = _playerSize,
            Delta = _currentVelocity * GameTime.FixedDeltaTime,
        };

        if (EnableCollision)
            _collisionHandler.VoxelCollisionsDynamic(ref playerAABB);
        else
            playerAABB.Position += playerAABB.Delta;

            _currentPosition = playerAABB.Position;
        _currentVelocity = playerAABB.Delta / GameTime.FixedDeltaTime;
    }

    public void UpdateGrounded()
    {
        StaticAABB playerAABB = new StaticAABB()
        {
            Position = _currentPosition,
            Size = _playerSize,
        };
        _groundedState = _collisionHandler.UpdateGrounded(playerAABB);
    }

    private void UpdateVelocity()
    {
        if (EnableGravity)
            _currentVelocity.Y -= GameTime.FixedDeltaTime * Gravity;

        _currentVelocity = _currentVelocity.ClampMagnitude(MAX_STABLE_MOVE_SPEED);
    }

    public bool IsOverlapping(Func<VoxelType, bool> func)
    {
        StaticAABB playerAABB = new StaticAABB()
        {
            Position = _currentPosition,
            Size = _playerSize,
        };
        return _collisionHandler.IsOverlapping(playerAABB, func);
    }
}
