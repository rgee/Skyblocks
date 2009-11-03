using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;


namespace Sky
{
    /// <summary>
    /// A player-controlled character in the world. Movement
    /// is controlled by the WASD keys.
    /// W = Move forward
    /// A = Turn left
    /// S = Move back
    /// D = Turn right
    /// 
    /// NOTE: Movement soon to be re-implemented as a JigLibX controller.
    /// </summary>
    class Player : Entity
    {
        private Effect effect;

        // Constants for motion
        private const float ROTATIONSPEED = 1f / 60f;
        private const float MOVEMENTSPEED = 1000f/60f;

        Texture shipTex;

        private Vector3 velocity = Vector3.Zero;

        /// <summary>
        /// Turn amount. Used to create rotation.
        /// </summary>
        private float yaw;
        public float Yaw
        {
            get { return yaw; }
        }

        public Player(GraphicsDevice device)
        {
            this.device = device;
            scalingMatrix = Matrix.CreateScale(1);
            position = new Vector3(0, 200, 0);
        }

        /// <summary>
        /// Load content for this player.
        /// </summary>
        /// <param name="model"></param>
        public void LoadContent(Model model, Effect effect, ContentManager content)
        {
            Debug.Assert(model != null);
            this.model = model;
            this.effect = effect;
            shipTex = content.Load<Texture>("Textures/PlayerMarineDiffuse");
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            Matrix rotation = Matrix.Identity;

            if (keyboardState.IsKeyDown(Keys.A))
            {
                yaw += ROTATIONSPEED;
                yaw = yaw % MathHelper.TwoPi;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                yaw -= ROTATIONSPEED;
                yaw = yaw % MathHelper.TwoPi;

                this.rotationMatrix *= Matrix.CreateFromAxisAngle(this.rotationMatrix.Up, MathHelper.Pi);
            }
            if (keyboardState.IsKeyDown(Keys.W))
            {
                /*
                Matrix forwardMovement = Matrix.CreateRotationY(yaw);
                Vector3 v = new Vector3(0, 0, MOVEMENTSPEED);
                v = Vector3.Transform(v, forwardMovement);
                position.Z += v.Z;
                position.X += v.X;
                */
                this.position -= this.rotationMatrix.Right * MOVEMENTSPEED;
                
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                /*
                Matrix forwardMovement = Matrix.CreateRotationY(yaw);
                Vector3 v = new Vector3(0, 0, -MOVEMENTSPEED);
                v = Vector3.Transform(v, forwardMovement);
                position.Z += v.Z;
                position.X += v.X;
                */

                this.position += this.rotationMatrix.Right * MOVEMENTSPEED;
            }

        }

        public override void Draw(Matrix view, Matrix projection)
        {
            worldMatrix = Matrix.CreateRotationY(yaw) * Matrix.CreateTranslation(this.position);

            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyBoneTransformsTo(transforms);

            effect.CurrentTechnique = effect.Techniques["Textured"];
            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                foreach(ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        
                        effect.Parameters["World"].SetValue(worldMatrix);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(transforms[mesh.ParentBone.Index]*worldMatrix)));
                        effect.Parameters["ModelTexture"].SetValue(shipTex);
                        
                        device.Vertices[0].SetSource(mesh.VertexBuffer, part.StreamOffset, part.VertexStride);
                        device.Indices = mesh.IndexBuffer;
                        device.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                                                                      part.BaseVertex, 0, part.NumVertices,
                                                                      part.StartIndex, part.PrimitiveCount);
                         
                    }
                }
                pass.End();
            }
            effect.End();

            /*
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = scalingMatrix * transforms[mesh.ParentBone.Index] * worldMatrix;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
             */
            base.Draw(view, projection);
        }
    }
}
