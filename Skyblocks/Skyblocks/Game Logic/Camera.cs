using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skyblocks
{
    public class Camera
    {
        private Vector3 position;
        /// <summary>
        /// The camera's position
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        
        private Matrix viewMatrix;
        /// <summary>
        /// The view matrix for this camera.
        /// </summary>
        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
        }

        private Matrix projectionMatrix;
        /// <summary>
        /// The projection matrix.
        /// </summary>
        public Matrix ProjectionMatrix
        {
            get { return projectionMatrix; }
        }

        private float distance;
        /// <summary>
        /// Distance from the board.
        /// </summary>
        public float Distance
        {
            get { return distance; }
        }

        private float currentHRotationAngle = 0.0f;
        private float targetHRotationAngle;
        private float currentVRotationAngle = 0.0f;
        private float targetVRotationAngle;

        private float cameraRotateCurrentTime;
        private float cameraRotateTotalTime;


        public enum ShiftState
        {
            Right,
            Left,
            Up,
            Down
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="distance">Distance from the board. Should be calculated based on board size.</param>
        public Camera(int boardWidth, int boardHeight, float aspectRatio)
        {
            float distance = ((float)Math.Max(boardWidth, boardHeight) * 400.0f / 5.0f);

            this.distance = distance / 100;
            Trace.WriteLine(this.distance.ToString());
            position = new Vector3(0.0f, 0.0f, this.distance);
            viewMatrix = Matrix.CreateLookAt(position, Vector3.Zero, Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f),
                aspectRatio, 1.0f, 1000.0f);
        }

        public void Update(GameTime gameTime)
        {
            
            if (cameraRotateCurrentTime >= cameraRotateTotalTime)
            {
                Trace.WriteLine("Done animating");
                currentHRotationAngle = currentVRotationAngle = 0.0f;
                cameraRotateCurrentTime = 0.0f;
                cameraRotateTotalTime = 0.0f;
            }
            else if (cameraRotateTotalTime != 0.0f)
            {
                cameraRotateCurrentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                Trace.WriteLine("Animating");
                float fraction = cameraRotateCurrentTime / cameraRotateTotalTime;
                currentHRotationAngle = MathHelper.SmoothStep(currentHRotationAngle, targetHRotationAngle, fraction);

                position = Vector3.Transform(position, Matrix.CreateRotationY(MathHelper.ToRadians(currentHRotationAngle)) *
                                                       Matrix.CreateRotationX(MathHelper.ToRadians(currentVRotationAngle)));

                viewMatrix = Matrix.CreateLookAt(position, Vector3.Zero, Vector3.Up);
            }

        }

        public void TurnBoard(ShiftState state)
        {
            cameraRotateTotalTime = 0.6f;
            switch (state)
            {
                case ShiftState.Up:
                    targetVRotationAngle = 90f;
                    break;
                case ShiftState.Down:
                    targetVRotationAngle = -90f;
                    break;
                case ShiftState.Right:
                    targetHRotationAngle = 90f;
                    break;
                case ShiftState.Left:
                    targetHRotationAngle = -90f;
                    break;
            }
        }
    }
}
