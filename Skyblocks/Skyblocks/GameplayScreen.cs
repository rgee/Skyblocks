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


        private Camera camera;
        /// <summary>
        /// The current camera being used.
        /// </summary>
        public Camera Camera
        {
            get { return camera; }
        }

        /// <summary>
        /// The 2-Dimensional board itself.
        /// </summary>
        private Board2D board;
        
        /// <summary>
        /// The world matrix for the board.
        /// </summary>
        private Matrix world = Matrix.CreateTranslation(0, 0, 0);
        

        private ContentManager content;
        /// <summary>
        /// A content manager to pass to underlying game objects.
        /// </summary>
        public ContentManager Content
        {
            get { return content; }
        }

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            board = new Board2D(10, 10, this);
            camera = new Camera(board.Width, board.Height, 1280f / 720f);
        }


        public override void LoadContent()
        {
            if (content == null)
            {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            }

            board.LoadContent();
        }
        public override void Draw(GameTime gameTime)
        {
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

            if (input.IsSelectLeft(ControllingPlayer))
            {
                board.SelectLeft();
            }

            if (input.IsSelectRight(ControllingPlayer))
            {
                board.SelectRight();
            }

            if (input.IsSelectUp(ControllingPlayer))
            {
                board.SelectUp();
            }

            if (input.IsSelectDown(ControllingPlayer))
            {
                board.SelectDown();
            }

            if (input.IsSwapLeft(ControllingPlayer))
            {
                board.SwapLeft();
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            board.Update(gameTime);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

    }
}
