using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sky
{
    public abstract class Camera
    {
        protected Matrix view;
        public Matrix View
        {
            get { return view; }
        }

        protected Matrix projection;
        public Matrix Projection
        {
            get { return projection; }
        }

        protected BoundingFrustum frustum;
        public BoundingFrustum Frustum
        {
            get { return frustum; }
        }

        protected Vector3 position;
        public Vector3 Position
        {
            get { return position; }
        }

        protected Viewport viewPort;

        public Camera(float viewAngle, Viewport viewPort, float nearPlaneDist, float farPlaneDist)
        {
            this.viewPort = viewPort;
            this.projection = Matrix.CreatePerspectiveFieldOfView(viewAngle, this.viewPort.AspectRatio, nearPlaneDist, farPlaneDist);
        }

        public abstract void Init(Vector3 startPosition, float hRotation, float vRotation);

        public abstract void Update(GameTime gameTime);

        public abstract void AddToPosition(Vector3 toAdd);
    }
}
