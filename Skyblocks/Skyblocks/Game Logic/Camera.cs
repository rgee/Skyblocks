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
            get
            {
                return Matrix.CreateLookAt(CurrentPosition, new Vector3(0, 0, 0), Vector3.Up);
            }
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

        /// <summary>
        /// Positions to move between during transitions
        /// </summary>
        private Vector3 destPosition, prevPosition;

        /// <summary>
        /// Counter for transition length.
        /// </summary>
        private float transitionAmount;

        /// <summary>
        /// Get the current position of the camera.
        /// </summary>
        Vector3 CurrentPosition
        {
            get
            {
                return Vector3.Lerp(prevPosition, destPosition, transitionAmount);
            }
        }

        /// <summary>
        /// Describes the direction the camera is supposed to shift in.
        /// </summary>
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

            prevPosition = new Vector3(0.0f, 0.0f, this.distance);
            destPosition = prevPosition;

            position = new Vector3(0.0f, 0.0f, this.distance);
            viewMatrix = Matrix.CreateLookAt(position, Vector3.Zero, Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f),
                aspectRatio, 1.0f, 1000.0f);
        }

        /// <summary>
        /// Update logic
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (transitionAmount < 1.0f)
                transitionAmount += 0.02f;

        }

        /// <summary>
        /// Rotates the camera about theboard in the specifed direction.
        /// </summary>
        /// <param name="state">The direction in which to turn.</param>
        public void TurnBoard(ShiftState state)
        {

            switch (state)
            {
                case ShiftState.Up:
                    prevPosition = destPosition;
                    destPosition = Vector3.Transform(CurrentPosition, Matrix.CreateRotationX(MathHelper.ToRadians(-90)));
                    transitionAmount = 0.0f;
                    break;
                case ShiftState.Down:
                    prevPosition = destPosition;
                    destPosition = Vector3.Transform(CurrentPosition, Matrix.CreateRotationX(MathHelper.ToRadians(90)));
                    transitionAmount = 0.0f;
                    break;
                case ShiftState.Right:
                    prevPosition = destPosition;
                    destPosition = Vector3.Transform(CurrentPosition, Matrix.CreateRotationY(MathHelper.ToRadians(90)));
                    transitionAmount = 0.0f;
                    break;
                case ShiftState.Left:
                    prevPosition = destPosition;
                    destPosition = Vector3.Transform(CurrentPosition, Matrix.CreateRotationY(MathHelper.ToRadians(-90)));
                    transitionAmount = 0.0f;
                    break;
            }



        }
    }
}
