﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="distance">Distance from the board. Should be calculated based on board size.</param>
        public Camera(float distance, float aspectRatio)
        {
            this.distance = distance;
            position = new Vector3(0.0f, 0.0f, distance);
            viewMatrix = Matrix.CreateLookAt(position, Vector3.Zero, Vector3.Up);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f),
                aspectRatio, 1.0f, 1000.0f);
        }
    }
}
