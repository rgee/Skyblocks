﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Skyblocks
{
    /// <summary>
    /// Represents a single block on the board.
    /// </summary>
    public class Block
    {
        private Model model;
        /// <summary>
        /// Getter for the model for the purpose of measurements.
        /// </summary>
        public Model Model
        {
            get { return model; }
        }


        private bool isActive = true;
        /// <summary>
        /// Is this block still on the board?
        /// </summary>
        public bool IsActive
        {
            get { return isActive; }
        }

        private Matrix world;
        /// <summary>
        /// The world matrix for this block.
        /// </summary>
        public Matrix World
        {
            get { return world; }
            set { world = value; }
        }


        private int xLayoutPosition;
        /// <summary>
        /// This piece's X position on the grid layout.
        /// </summary> 
        public int XLayoutPosition
        {
            get { return xLayoutPosition; }
            set { xLayoutPosition = value; }
        }

        private int yLayoutPosition;
        /// <summary>
        /// This piece's Y position on the gird layout.
        /// </summary>
        public int YLayoutPosition
        {
            get { return yLayoutPosition; }
            set { yLayoutPosition = value; }
        }

        public Block()
        {
            
        }

        public void LoadContent(ContentManager content)
        {
            model = content.Load<Model>("Models//Cats");
        }

        public void Draw(Camera cam, GameTime gameTime)
        {
            if (isActive)
            {
                Matrix[] transforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        effect.World = world * transforms[mesh.ParentBone.Index] *
                                        Matrix.CreateRotationX(MathHelper.ToRadians(90)) *
                                        Matrix.CreateScale(new Vector3(1.0f, 1.0f, 0.1f));
                        effect.View = cam.ViewMatrix;
                        effect.Projection = cam.ProjectionMatrix;
                    }
                    mesh.Draw();
                }
            }
        }
    }
}
