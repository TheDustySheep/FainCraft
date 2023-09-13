using FainCraft.Gameplay.WorldScripts.Data;
using FainEngine_v2.Core.GameObjects;
using System.Numerics;

namespace FainCraft.Gameplay.Motors;
internal class EntityMotor
{
    #region Public Properties
    public GroundedState groundedState => _internalMotor.GroundedState;

    public float Gravity
    {
        get => _internalMotor.Gravity;
        set => _internalMotor.Gravity = value;
    }

    private Vector3 _playerSize = new Vector3(0.4f, 1.8f, 0.4f);
    public Vector3 PlayerSize
    {
        get => _playerSize;
        set 
        {
            _playerSize = value;
            _internalMotor.Size = value; 
        }
    }

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
        _internalMotor.Size = _playerSize;
    }

    public void FixedUpdate()
    {
        _internalMotor.FixedUpdate();
    }

    public void Update()
    {
        PositionStart = _internalMotor.InterpolatePosition();
    }
}
