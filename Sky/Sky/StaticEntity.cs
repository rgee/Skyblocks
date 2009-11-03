using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sky
{
    /// <summary>
    /// Defines an entity within the world with a position and a model representation
    /// that will never move.
    /// </summary>
    public class StaticEntity : Entity
    {



        public StaticEntity()
        {
            position = new Vector3(0, -10, 0);
        }

        public void LoadContent(Model model, GraphicsDevice device)
        {
            Debug.Assert(model != null);

            this.device = device;
            this.model = model;
            scalingMatrix = Matrix.CreateScale(.2f);
        }

        public override void Update(GameTime gameTime)
        {
            //translationMatrix = Matrix.CreateTranslation(position);
            //worldMatrix = scalingMatrix * rotationMatrix * translationMatrix;

            base.Update(gameTime);
        }

        public override void Draw(Matrix view, Matrix projection)
        {
            device.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            device.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    
                    effect.EnableDefaultLighting();
                    effect.World = worldMatrix;
                    effect.View = view;
                    effect.Projection = projection;
                    
                }
                mesh.Draw();
            }


            base.Draw(view, projection);
        }
    }
}
