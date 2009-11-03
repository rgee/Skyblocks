using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Sky
{
    /// <summary>
    /// This class represents a "sky" made up of
    /// six textures. (A TextureCube)
    /// </summary>
    class Skydome
    {
        GraphicsDevice device;
        Model skyBoxModel;
        TextureCube skyBoxTexture;

        public Skydome()
        {
        }

        public void LoadContent(GraphicsDevice device, Model skyBoxModel, TextureCube skyBoxTexture)
        {
            this.device = device;
            this.skyBoxModel = skyBoxModel;
            this.skyBoxTexture = skyBoxTexture;
        }

        public void Draw(FirstPersonCamera cam)
        {
            device.RenderState.DepthBufferWriteEnable = false;
            foreach (ModelMesh mesh in skyBoxModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.Parameters["Texture0"].SetValue(skyBoxTexture);
                    effect.Parameters["matView"].SetValue(cam.View);
                    effect.Parameters["matProj"].SetValue(cam.Projection);
                }
                mesh.Draw();
            }
            device.RenderState.DepthBufferWriteEnable = true;

            // Temporarily set to false in shader.
            device.RenderState.DepthBufferEnable = true;
        }
    }
}
