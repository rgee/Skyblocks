using System;
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

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }



        Matrix destination, prevLocation;

        public Matrix Destination
        {
            get { return destination; }
            set { destination = value; }
        }
        public Matrix PrevLocation
        {
            get { return prevLocation; }
            set { prevLocation = value; }
        }


        float transitionAmount = 1.0f;
        public float TransitionAmount
        {
            get { return transitionAmount; }
            set { transitionAmount = value; }
        }

        public Matrix CurrentWorld
        {
            get
            {
                return Matrix.Lerp(prevLocation, destination, transitionAmount);
            }
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
                        effect.World = CurrentWorld * transforms[mesh.ParentBone.Index] *
                                        Matrix.CreateRotationX(MathHelper.ToRadians(90)) *
                                        Matrix.CreateScale(new Vector3(1.0f, 1.0f, 0.1f));
                        if (isSelected)
                        {
                            effect.AmbientLightColor = new Vector3(0.9f, 0.9f, 0.9f);
                        }
                        else
                        {
                            effect.AmbientLightColor = new Vector3(0f, 0f, 0f);
                        }
                        effect.View = cam.ViewMatrix;
                        effect.Projection = cam.ProjectionMatrix;
                    }
                    mesh.Draw();
                }
            }
        }

        public void Update()
        {
            if (transitionAmount < 1.0f)
                transitionAmount += 0.05f;
            if (transitionAmount > 1.0f)
                transitionAmount = 1.0f;
        }

    }
}
