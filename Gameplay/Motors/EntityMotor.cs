using FainCraft.Gameplay.WorldScripts.Data;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Physics.AABB;
using System.Numerics;

namespace FainCraft.Gameplay.Motors;
internal class EntityMotor
{
    #region Public Properties
    public GroundedState groundedState => _internalMotor.groundedState;

    public float Gravity
    {
        get => _internalMotor.Gravity;
        set => _internalMotor.Gravity = value;
    }

    public Vector3 PlayerSize
    {
        get;
        set;
    } = new Vector3(0.4f, 1.8f, 0.4f);

    public Vector3 PositionStart
    {
        get => _transform.LocalPosition - (PlayerSize / 2);
        set => _transform.LocalPosition = (PlayerSize / 2) + value;
    }

    public Vector3 Velocity
    {
        get => _internalMotor.Velocity;
        set => _internalMotor.Velocity = value;
    }
    #endregion

    readonly EntityMotorInternal _internalMotor;
    readonly Transform _transform;

    public EntityMotor(Transform transform, IWorldData worldData)
    {
        _transform = transform;
        _internalMotor = new EntityMotorInternal(new CollisionHandler(worldData), PositionStart);
    }

    public void SetPosition(Vector3 position) => _internalMotor.SetPosition(position);

    public void FixedUpdate()
    {
        _internalMotor.FixedUpdate(new StaticAABB()
        {
            Position = PositionStart,
            Size = PlayerSize
        });
    }

    public void Update()
    {
        PositionStart = _internalMotor.InterpolatePosition();
    }
}
