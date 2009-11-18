using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace Skyblocks
{
    public class GameplayScreen : GameScreen
    {
        Effect effect;
        Model helicopter;
        Texture2D helicopterDiffuseMap;
        Texture2D helicopterNormalMap;

        Matrix world = Matrix.CreateTranslation(0, 0, 0);
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1280f / 720f, 0.1f, 1000f);
        float angle = 0;
        float distance = 10;

        Vector3 viewVector;


        ContentManager content;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            if (content == null)
            {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            }

            effect = content.Load<Effect>("Shaders//NormalMap");
            helicopter = content.Load<Model>("Models//Helicopter");
            helicopterDiffuseMap = content.Load<Texture2D>("Models//HelicopterTexture");
            helicopterNormalMap = content.Load<Texture2D>("Textures//HelicopterNormalMap");
        }
        public override void Draw(GameTime gameTime)
        {
            foreach (ModelMesh mesh in helicopter.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    Matrix worldInverseTranspose = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                    part.Effect = effect;
                    effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTranspose);
                    effect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["ViewVector"].SetValue(viewVector);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["AmbientColor"].SetValue(Color.CornflowerBlue.ToVector4());
                    effect.Parameters["AmbientIntensity"].SetValue(0.5f);
                    effect.Parameters["ModelTexture"].SetValue(helicopterDiffuseMap);
                    effect.Parameters["NormalMap"].SetValue(helicopterNormalMap);
                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

    }
}
