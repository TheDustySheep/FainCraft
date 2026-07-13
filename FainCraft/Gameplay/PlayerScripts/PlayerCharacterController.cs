using FainCraft.Gameplay.Motors;
using FainCraft.Gameplay.PlayerScripts.Gamemodes;
using FainEngine_v2.Core;
using FainEngine_v2.Entities;
using FainEngine_v2.Extensions;
using FainEngine_v2.Utils;
using Silk.NET.Input;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts;
internal class PlayerCharacterController
{
    readonly Transform _camTransform;
    readonly EntityMotor _motor;

    readonly float lookSensitivity = 0.1f;
    Vector2 CameraRotation;

    StateMachine<Vector2> _gamemodeStateMachine;

    private readonly Survival _survival;
    private readonly NoClip   _noclip;

    private readonly IGameInputs _gameInputs;

    public PlayerCharacterController(Transform camTransform, EntityMotor motor)
    {
        _gameInputs = DependencyInjector.Resolve<IGameInputs>();

        _camTransform = camTransform;
        _motor = motor;

        _survival = new Survival(motor, camTransform);
        _noclip   = new NoClip  (motor, camTransform);

        _gamemodeStateMachine = new StateMachine<Vector2>(
            _survival
        );
    }

    public void Update()
    {
        UpdateGamemode();
        UpdateRotation();
        UpdatePosition();

        if (_gameInputs.IsKeyDown(Key.Escape))
            _gameInputs.ExitProgram();
    }

    private void UpdateGamemode()
    {
        if (_gameInputs.IsKeyDown(Key.G))
            _gamemodeStateMachine.EnterState(_survival);
        else if (_gameInputs.IsKeyDown(Key.H))
            _gamemodeStateMachine.EnterState(_noclip);
    }

    private void UpdatePosition()
    {
        _gamemodeStateMachine.Tick(MovementInputs());
    }

    private void UpdateRotation()
    {
        var mouseDelta = _gameInputs.MouseDelta;

        if (mouseDelta != default)
        {
            CameraRotation += mouseDelta * lookSensitivity;

            CameraRotation.Y = Math.Clamp(CameraRotation.Y, -89.0f, 89.0f);
            _camTransform.LocalRotation = Quaternion.CreateFromYawPitchRoll
            (
                MathUtils.DegreesToRadians(CameraRotation.X),
                MathUtils.DegreesToRadians(CameraRotation.Y),
                0
            );
        }
    }

    #region Movement Inputs
    private Vector2 MovementInputs()
    {
        Vector2 input = Vector2.Zero;

        if (_gameInputs.IsKeyHeld(Key.W))
            input.Y += 1f;
        if (_gameInputs.IsKeyHeld(Key.S))
            input.Y -= 1f;

        if (_gameInputs.IsKeyHeld(Key.A))
            input.X -= 1f;
        if (_gameInputs.IsKeyHeld(Key.D))
            input.X += 1f;

        return input.Normalized();
    }
    #endregion
}
