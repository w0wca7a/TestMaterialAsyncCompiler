﻿using System;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;

namespace TestMaterialAsyncCompiler;

/// <summary>
/// A script that allows to move and rotate an entity through keyboard, mouse and touch input to provide basic camera navigation.
/// </summary>
/// <remarks>
/// The entity can be moved using W, A, S, D, Q and E, arrow keys, a gamepad's left stick or dragging/scaling using multi-touch.
/// Rotation is achieved using the Numpad, the mouse while holding the right mouse button, a gamepad's right stick, or dragging using single-touch.
/// </remarks>
public class BasicCameraController : SyncScript
{
    private const float MaximumPitch = MathUtil.PiOverTwo * 0.99f;

    private Vector3 upVector;
    private Vector3 translation;
    private float yaw;
    private float pitch;

    //public bool Gamepad { get; set; } = false;

    public Vector3 KeyboardMovementSpeed { get; set; } = new Vector3(5.0f);

    //public Vector3 TouchMovementSpeed { get; set; } = new Vector3(0.7f, 0.7f, 0.3f);

    public float SpeedFactor { get; set; } = 5.0f;

    //public Vector2 KeyboardRotationSpeed { get; set; } = new Vector2(3.0f);

    public Vector2 MouseRotationSpeed { get; set; } = new Vector2(1.0f, 1.0f);

    //public Vector2 TouchRotationSpeed { get; set; } = new Vector2(1.0f, 0.7f);

    public override void Start()
    {
        base.Start();

        // Default up-direction
        upVector = Vector3.UnitY;

        // Configure touch input
        if (!Platform.IsWindowsDesktop)
        {
            Input.Gestures.Add(new GestureConfigDrag());
            Input.Gestures.Add(new GestureConfigComposite());
        }
    }

    public override void Update()
    {
        ProcessInput();
        UpdateTransform();
    }

    private void ProcessInput()
    {
        float deltaTime = (float)Game.UpdateTime.Elapsed.TotalSeconds;
        translation = Vector3.Zero;
        yaw = 0f;
        pitch = 0f;

        // Keyboard and Gamepad based movement
        {
            float speed = 0.1f * deltaTime;

            Vector3 dir = Vector3.Zero;
            if (Input.HasKeyboard)
            {
                // Move with keyboard
                // Forward/Backward
                if (Input.IsKeyDown(Keys.W) || Input.IsKeyDown(Keys.Up))
                {
                    dir.Z += 1;
                }
                if (Input.IsKeyDown(Keys.S) || Input.IsKeyDown(Keys.Down))
                {
                    dir.Z -= 1;
                }

                // Left/Right
                if (Input.IsKeyDown(Keys.A) || Input.IsKeyDown(Keys.Left))
                {
                    dir.X -= 1;
                }
                if (Input.IsKeyDown(Keys.D) || Input.IsKeyDown(Keys.Right))
                {
                    dir.X += 1;
                }

                // Down/Up
                if (Input.IsKeyDown(Keys.Q))
                {
                    dir.Y -= 1;
                }
                if (Input.IsKeyDown(Keys.E))
                {
                    dir.Y += 1;
                }

                // Increase speed when pressing shift
                if (Input.IsKeyDown(Keys.LeftShift) || Input.IsKeyDown(Keys.RightShift))
                {
                    speed *= SpeedFactor;
                }

                if (dir.Length() > 1f)
                {
                    dir = Vector3.Normalize(dir);
                }
            }

            // Finally, push all of that to the translation variable which will be used within UpdateTransform()
            translation += dir * KeyboardMovementSpeed * speed;
        }

        // Mouse movement and gestures
        {
            if (Input.HasMouse)
            {
                // Rotate with mouse
                if (Input.IsMouseButtonDown(MouseButton.Right))
                {
                    Input.LockMousePosition();
                    Game.IsMouseVisible = false;

                    yaw -= Input.MouseDelta.X * MouseRotationSpeed.X;
                    pitch -= Input.MouseDelta.Y * MouseRotationSpeed.Y;
                }
                else
                {
                    Input.UnlockMousePosition();
                    Game.IsMouseVisible = true;
                }
            }
        }
    }

    private void UpdateTransform()
    {
        // Get the local coordinate system
        var rotation = Matrix.RotationQuaternion(Entity.Transform.Rotation);

        // Enforce the global up-vector by adjusting the local x-axis
        var right = Vector3.Cross(rotation.Forward, upVector);
        var up = Vector3.Cross(right, rotation.Forward);

        // Stabilize
        right.Normalize();
        up.Normalize();

        // Adjust pitch. Prevent it from exceeding up and down facing. Stabilize edge cases.
        var currentPitch = MathUtil.PiOverTwo - MathF.Acos(Vector3.Dot(rotation.Forward, upVector));
        pitch = MathUtil.Clamp(currentPitch + pitch, -MaximumPitch, MaximumPitch) - currentPitch;

        Vector3 finalTranslation = translation;
        finalTranslation.Z = -finalTranslation.Z;
        finalTranslation = Vector3.TransformCoordinate(finalTranslation, rotation);

        // Move in local coordinates
        Entity.Transform.Position += finalTranslation;

        // Yaw around global up-vector, pitch and roll in local space
        Entity.Transform.Rotation *= Quaternion.RotationAxis(right, pitch) * Quaternion.RotationAxis(upVector, yaw);
    }
}
