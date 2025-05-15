using FainCraft.Gameplay.Motors;
using FainCraft.Gameplay.PlayerScripts.Gamemodes;
using FainEngine_v2.Core;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Extensions;
using FainEngine_v2.Utils;
using Silk.NET.Input;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts;
internal class PlayerCharacterController
{
    readonly Transform camTransform;
    readonly EntityMotor motor;

    readonly float lookSensitivity = 0.1f;
    Vector2 CameraRotation;

    StateMachine<Vector2> _gamemodeStateMachine;

    private readonly Survival _survival;
    private readonly NoClip   _noclip;

    public PlayerCharacterController(Transform camTransform, EntityMotor motor)
    {
        this.camTransform = camTransform;
        this.motor = motor;

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

        if (GameInputs.IsKeyDown(Key.Escape))
            GameInputs.ExitProgram();
    }

    private void UpdateGamemode()
    {
        if (GameInputs.IsKeyDown(Key.G))
            _gamemodeStateMachine.EnterState(_survival);
        else if (GameInputs.IsKeyDown(Key.H))
            _gamemodeStateMachine.EnterState(_noclip);
    }

    private void UpdatePosition()
    {
        _gamemodeStateMachine.Tick(MovementInputs());
    }

    private void UpdateRotation()
    {
        var mouseDelta = GameInputs.MouseDelta;

        if (mouseDelta != default)
        {
            CameraRotation += mouseDelta * lookSensitivity;

            CameraRotation.Y = Math.Clamp(CameraRotation.Y, -89.0f, 89.0f);
            camTransform.LocalRotation = Quaternion.CreateFromYawPitchRoll
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

        if (GameInputs.IsKeyHeld(Key.W))
            input.Y += 1f;
        if (GameInputs.IsKeyHeld(Key.S))
            input.Y -= 1f;

        if (GameInputs.IsKeyHeld(Key.A))
            input.X -= 1f;
        if (GameInputs.IsKeyHeld(Key.D))
            input.X += 1f;

        return input.Normalized();
    }
    #endregion
}
