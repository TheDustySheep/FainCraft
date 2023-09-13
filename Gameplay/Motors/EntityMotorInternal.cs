using FainEngine_v2.Core;
using FainEngine_v2.Extensions;
using FainEngine_v2.Physics.AABB;
using FainEngine_v2.Utils;
using System.Numerics;

namespace FainCraft.Gameplay.Motors;
internal class EntityMotorInternal
{
    public Vector3 Velocity
    {
        get => _currentVelocity;
        set => _currentVelocity = value;
    }

    public GroundedState GroundedState;
    public Vector3 Size;
    public float Gravity = 32f;

    private const float MAX_STABLE_MOVE_SPEED = CollisionHandler.IterationCount / GameTime.FixedDeltaTime;

    CollisionHandler _collisionHandler;

    float _lastTime;

    Vector3 _currentPosition;
    Vector3 _lastPosition;
    Vector3 _currentVelocity;


    public EntityMotorInternal(CollisionHandler collisionHandler, Vector3 startPosition)
    {
        _collisionHandler = collisionHandler;
        _currentPosition = startPosition;
        _lastPosition = startPosition;
        _lastTime = GameTime.TotalTime;
    }

    #region Public Functions
    public Vector3 InterpolatePosition()
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
            Size = Size,
            Delta = _currentVelocity * GameTime.FixedDeltaTime,
        };

        _collisionHandler.VoxelCollisionsDynamic(ref playerAABB);

        _currentPosition = playerAABB.Position;
        _currentVelocity = playerAABB.Delta / GameTime.FixedDeltaTime;
    }

    public void UpdateGrounded()
    {
        StaticAABB playerAABB = new StaticAABB()
        {
            Position = _currentPosition,
            Size = Size,
        };
        GroundedState = _collisionHandler.UpdateGrounded(playerAABB);
    }

    private void UpdateVelocity()
    {
        _currentVelocity.Y -= GameTime.FixedDeltaTime * Gravity;

        _currentVelocity = _currentVelocity.ClampMagnitude(MAX_STABLE_MOVE_SPEED);
    }
}
