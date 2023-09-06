using FainEngine_v2.Core;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Extensions;
using FainEngine_v2.Rendering.Cameras;
using FainEngine_v2.Utils;
using Silk.NET.Input;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts;
internal class CameraController
{
    Camera3D camera;
    Transform camTransform;
    Transform playerTransform;

    private float moveSpeed = 10f;
    private float lookSensitivity = 0.1f;
    private Vector2 CameraRotation;

    public CameraController(Camera3D camera, Transform camTransform, Transform playerTransform)
    {
        this.camera = camera;
        this.camTransform = camTransform;
        this.playerTransform = playerTransform;
    }

    public void Update()
    {
        UpdatePosition();
        UpdateRotation();
        UpdateZoom();
        UpdateMatrix();
    }

    private void UpdateMatrix()
    {
    }

    private void UpdatePosition()
    {
        Vector3 targetDelta = Vector3.Zero;

        var cameraForward = camTransform.Forward;
        var cameraRight = camTransform.Right;

        if (GameInputs.IsKeyHeld(Key.W))
        {
            //Move forwards
            targetDelta += cameraForward;
        }
        if (GameInputs.IsKeyHeld(Key.S))
        {
            //Move backwards
            targetDelta -= cameraForward;
        }
        if (GameInputs.IsKeyHeld(Key.A))
        {
            //Move left
            targetDelta -= Vector3.Normalize(cameraRight);
        }
        if (GameInputs.IsKeyHeld(Key.D))
        {
            //Move right
            targetDelta += Vector3.Normalize(cameraRight);
        }

        targetDelta.Y = 0f;
        if (targetDelta != default)
        {
            targetDelta = targetDelta.Normalize();
        }

        if (GameInputs.IsKeyHeld(Key.Space))
        {
            targetDelta += Vector3.UnitY;
        }
        if (GameInputs.IsKeyHeld(Key.ControlLeft))
        {
            targetDelta -= Vector3.UnitY;
        }

        playerTransform.Position += targetDelta * moveSpeed * GameTime.DeltaTime;

        if (GameInputs.IsKeyDown(Key.Escape))
            GameInputs.ExitProgram();
    }

    private void UpdateRotation()
    {
        var mouseDelta = GameInputs.MouseDelta;

        if (mouseDelta != default)
        {
            CameraRotation += mouseDelta * lookSensitivity;

            CameraRotation.Y = Math.Clamp(CameraRotation.Y, -89.0f, 89.0f);
            camTransform.Rotation = Quaternion.CreateFromYawPitchRoll
            (
                MathHelper.DegreesToRadians(CameraRotation.X),
                MathHelper.DegreesToRadians(CameraRotation.Y), 
                0
            );
        }
    }

    private void UpdateZoom()
    {
        //We don't want to be able to zoom in too close or too far away so clamp to these values
        moveSpeed = Math.Clamp(moveSpeed - GameInputs.ScrollDelta.Y * 0.1f, 1.0f, 20f);
    }
}
