using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sky
{
    /// <summary>
    /// This abstract class represents the base class for all visible entities within the world.
    /// </summary>
    public abstract class Entity
    {
        protected Matrix worldMatrix = Matrix.Identity;
        protected Matrix scalingMatrix = Matrix.Identity;
        protected Matrix rotationMatrix = Matrix.Identity;
        protected Matrix translationMatrix = Matrix.Identity;

        protected GraphicsDevice device;

        protected Vector3 position = Vector3.Zero;
        protected Model model;


        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Matrix RotationMatrix
        {
            get { return rotationMatrix; }
        }
        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }

        public virtual void Draw(Matrix view, Matrix projection)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }
}
