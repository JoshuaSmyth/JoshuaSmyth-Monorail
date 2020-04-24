using Monorail.Mathlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monorail.Platform
{
    public class GameCamera
    {
        public Matrix4 WorldMatrix = Matrix4.Identity;
        public Matrix4 ViewMatrix;
        public Matrix4 ProjMatrix;

        public Vector3 Position;

        public Vector3 Forward;

        public Vector3 Right;

        public Vector3 Up;

        public float Yaw;

        public float Pitch;

        public GameCamera(Vector3 position, Vector3 lookAt, float yaw, float pitch)
        {
            var aspect = GameWindow.ScreenWidth / (float)GameWindow.ScreenHeight;
            ProjMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.ToRads(60), aspect, 1f, 1000.0f);
            Position = position;

            Forward = Vector3.Normalize(lookAt - position);

            Pitch = pitch;
            Yaw = yaw;

            Update();
        }

        public void MoveForward(float v)
        {
            Position += Forward * v;
        }

        public void MoveRight(float v)
        {
            Position += Right * v;
        }

        public void RotateYaw(float increment)
        {
            Yaw += increment;
        }

        public void AdjustPitch(float increment)
        {
            Pitch += increment;
            if (Pitch > 85)
            {
                Pitch = 85f;
            }
            if (Pitch < -85)
            {
                Pitch = -85f;
            }
        }

        public void Update()
        {
            Forward.X = (float)(Math.Cos(MathHelper.ToRads(Yaw)) * Math.Cos(MathHelper.ToRads(Pitch)));
            Forward.Y = (float)Math.Sin(MathHelper.ToRads(Pitch));
            Forward.Z = (float)(Math.Sin(MathHelper.ToRads(Yaw)) * Math.Cos(MathHelper.ToRads(Pitch)));

            Forward = Vector3.Normalize(Forward);

            Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));
            Up = Vector3.Normalize(Vector3.Cross(Right, Forward));

            ViewMatrix = Matrix4.LookAt(Position, Position + Forward, Up);
        }

        /// <summary>
        /// Gets the lookat with the position removed
        /// </summary>
        public Matrix4 GetLookAtMatrix()
        {
            return Matrix4.LookAt(new Vector3(), new Vector3() + Forward, Up);
        }
    }
}
