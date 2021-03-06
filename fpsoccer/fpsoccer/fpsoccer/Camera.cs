﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using fpsoccer.Input;

namespace fpsoccer
{
    /// <summary>
    /// Basic camera class supporting mouse/keyboard/gamepad-based movement.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// Gets or sets the position of the camera.
        /// </summary>
        public Vector3 Position { get; set; }
        float yaw;
        float pitch;
        /// <summary>
        /// Gets or sets the yaw rotation of the camera.
        /// </summary>
        public float Yaw
        {
            get
            {
                return yaw;
            }
            set
            {
                yaw = MathHelper.WrapAngle(value);
            }
        }
        /// <summary>
        /// Gets or sets the pitch rotation of the camera.
        /// </summary>
        public float Pitch
        {
            get
            {
                return pitch;
            }
            set
            {
                pitch = MathHelper.Clamp(value, -MathHelper.PiOver2, MathHelper.PiOver2);
            }
        }

        /// <summary>
        /// Gets or sets the speed at which the camera moves.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// Gets the view matrix of the camera.
        /// </summary>
        public Matrix ViewMatrix { get; private set; }
        /// <summary>
        /// Gets or sets the projection matrix of the camera.
        /// </summary>
        public Matrix ProjectionMatrix { get; set; }

        /// <summary>
        /// Gets the world transformation of the camera.
        /// </summary>
        public Matrix WorldMatrix { get; private set; }

        /// <summary>
        /// Gets the game owning the camera.
        /// </summary>
        public FpsGame Game { get; private set; }

        /// <summary>
        /// Constructs a new camera.
        /// </summary>
        /// <param name="game">Game that this camera belongs to.</param>
        /// <param name="position">Initial position of the camera.</param>
        /// <param name="speed">Initial movement speed of the camera.</param>
        public Camera(FpsGame game, Vector3 position, float speed)
        {
            Game = game;
            Position = position;
            Speed = speed;
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                (float)game.Window.ClientBounds.Width /
                (float)game.Window.ClientBounds.Height,
                1, 2000);
            Mouse.SetPosition(200, 200);
        }

        /// <summary>
        /// Moves the camera forward using its speed.
        /// </summary>
        /// <param name="dt">Timestep duration.</param>
        public void MoveForward(float dt)
        {
            Position += new Vector3(WorldMatrix.Forward.X, 0, WorldMatrix.Forward.Z) * (dt * Speed);
        }
        /// <summary>
        /// Moves the camera right using its speed.
        /// </summary>
        /// <param name="dt">Timestep duration.</param>
        /// 
        public void MoveRight(float dt)
        {
            Position += WorldMatrix.Right * (dt * Speed);
        }
        /// <summary>
        /// Moves the camera up using its speed.
        /// </summary>
        /// <param name="dt">Timestep duration.</param>
        /// 
        public void MoveUp(float dt)
        {
            Position += new Vector3(0, (dt * Speed), 0);
        }

        public void Update(float gameTime, KeyboardWrapper keyboard, MouseWrapper mouse)
        {
#if XBOX360
            //Turn based on gamepad input.
            Yaw += Game.GamePadState.ThumbSticks.Right.X * -1.5f * dt;
            Pitch += Game.GamePadState.ThumbSticks.Right.Y * 1.5f * dt;
#else
            //Turn based on mouse input.
            Yaw += (200 - (mouse.State.X * (keyboard.InvertOptions.InvertX ? -1 : 1))) * gameTime * .12f;
            Pitch += (200 - (mouse.State.Y * (mouse.InvertOptions.InvertY ? -1 : 1))) * gameTime * .12f;
#endif
            Mouse.SetPosition(200, 200);

            WorldMatrix = Matrix.CreateFromAxisAngle(Vector3.Right, Pitch) * Matrix.CreateFromAxisAngle(Vector3.Up, Yaw);


            float distance = Speed * gameTime;
#if XBOX360
            //Move based on gamepad input.
                MoveForward(Game.GamePadState.ThumbSticks.Left.Y * distance);
                MoveRight(Game.GamePadState.ThumbSticks.Left.X * distance);
                if (Game.GamePadState.IsButtonDown(Buttons.LeftStick))
                    MoveUp(distance);
                if (Game.GamePadState.IsButtonDown(Buttons.RightStick))
                    MoveUp(-distance);
#else

            //Scoot the camera around depending on what keys are pressed.
            if (keyboard.State.IsKeyDown(KeyBindings.Up))
                MoveForward(distance);
            if (keyboard.State.IsKeyDown(KeyBindings.Down))
                MoveForward(-distance);
            if (keyboard.State.IsKeyDown(KeyBindings.Left))
                MoveRight(-distance);
            if (keyboard.State.IsKeyDown(KeyBindings.Right))
                MoveRight(distance);
#endif

            WorldMatrix = WorldMatrix * Matrix.CreateTranslation(Position);
            ViewMatrix = Matrix.Invert(WorldMatrix);
        }
    }
}
