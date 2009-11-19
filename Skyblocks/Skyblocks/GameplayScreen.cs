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

        private Camera camera;
        /// <summary>
        /// The current camera being used.
        /// </summary>
        public Camera Camera
        {
            get { return camera; }
        }
        Board2D board;
        
        Matrix world =Matrix.CreateTranslation(0, 0, 0);
        float angle = 0;
        float distance = 10;
        Vector3 viewVector;
        

        private ContentManager content;
        public ContentManager Content
        {
            get { return content; }
        }

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            board = new Board2D(20, 20, this);
            camera = new Camera(board.Width, board.Height, 1280f / 720f);
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
            board.LoadContent();
        }
        public override void Draw(GameTime gameTime)
        {
            /*
            foreach (ModelMesh mesh in helicopter.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    Matrix worldInverseTranspose = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                    part.Effect = effect;
                    effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTranspose);
                    effect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    effect.Parameters["View"].SetValue(camera.ViewMatrix);
                    effect.Parameters["ViewVector"].SetValue(viewVector);
                    effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
                    effect.Parameters["AmbientColor"].SetValue(Color.CornflowerBlue.ToVector4());
                    effect.Parameters["AmbientIntensity"].SetValue(0.5f);
                    effect.Parameters["ModelTexture"].SetValue(helicopterDiffuseMap);
                    effect.Parameters["NormalMap"].SetValue(helicopterNormalMap);
                }
                mesh.Draw();
            }
            */
            board.Draw(gameTime);
            base.Draw(gameTime);
        }

        public override void HandleInput(InputState input)
        {
            if(input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (input.IsPauseGame(ControllingPlayer))
            {
                ExitScreen();
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

    }
}
