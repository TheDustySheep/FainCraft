using FainEngine_v2.Core;
using FainEngine_v2.Extensions;
using FainEngine_v2.Physics.AABB;
using FainEngine_v2.Utils;
using System.Diagnostics;
using System.Numerics;

namespace FainCraft.Gameplay.Motors;
internal class EntityMotorInternal
{
    public GroundedState groundedState => _groundedState;

    public float Gravity = 50f;
    public Vector3 Velocity;

    CollisionHandler collisionHandler;

    Vector3? _setPositionRequest;

    Vector3 _nextPosition;
    Vector3 _startPosition;
    float _startTime;
    float _nextTime;

    Vector3 _playerSize;

    GroundedState _groundedState;

    public EntityMotorInternal(CollisionHandler collisionHandler, Vector3 startPosition)
    {
        this.collisionHandler = collisionHandler;
        _startPosition = startPosition;
        _nextPosition = startPosition;
        _startTime = GameTime.TotalTime;
        _nextTime = _startTime + GameTime.FixedDeltaTime;
    }

    #region Public Functions
    public Vector3 InterpolatePosition()
    {
        float t = MathUtils.InvLerp(_startTime, _nextTime, GameTime.TotalTime);
        return Vector3Extensions.Lerp(_startPosition, _nextPosition, MathUtils.Clamp01(t));
    }

    public void SetPosition(Vector3 newPosition) => _setPositionRequest = newPosition;

    public void FixedUpdate(StaticAABB player)
    {
        // Phase 1 setup and initial collisions
        UpdateInitialInternals(player);
        ProcessRequests();
        HandleCollisions();

        // Phase 2 setting state for next frame
        UpdateGrounded();
        UpdateVelocity();
        UpdateFinalInternals();
    }
    #endregion

    private void UpdateInitialInternals(StaticAABB player)
    {
        _playerSize = player.Size;
        _startPosition = player.Position;
        _startTime = GameTime.TotalTime;
    }

    private void ProcessRequests()
    {
        if (_setPositionRequest is not null)
            _startPosition = _setPositionRequest.Value;
    }

    private void HandleCollisions()
    {
        DynamicAABB playerAABB = new DynamicAABB()
        {
            Position = _startPosition,
            Size = _playerSize,
            Delta = Velocity * GameTime.FixedDeltaTime,
        };

        collisionHandler.VoxelCollisionsDynamic(ref playerAABB);

        _startPosition = playerAABB.Position;
        Velocity = playerAABB.Delta / GameTime.FixedDeltaTime;
    }

    private void UpdateGrounded()
    {
        StaticAABB playerAABB = new StaticAABB()
        {
            Position = _startPosition,
            Size = _playerSize,
        };
        _groundedState = collisionHandler.UpdateGrounded(playerAABB);
    }

    private void UpdateVelocity()
    {
        Velocity.Y -= GameTime.FixedDeltaTime * Gravity;
        //Velocity.Y = MathUtils.MoveTowards(Velocity.Y, -Gravity, GameTime.FixedDeltaTime * Gravity);
    }

    private void UpdateFinalInternals()
    {
        _nextPosition = _startPosition + (Velocity * GameTime.FixedDeltaTime);
        _nextTime = _startTime + GameTime.FixedDeltaTime;

        _setPositionRequest = null;
    }
}
