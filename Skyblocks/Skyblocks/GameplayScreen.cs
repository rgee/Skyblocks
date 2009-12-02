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

        private int dimensions;

        public GameplayScreen(int dimensions)
        {
            this.dimensions = dimensions;


            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            board = new Board2D(5, 5, this);
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

        public override void UnloadContent()
        {
            board.UnloadContent();

            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            /* Display error if 3d
            if (dimensions == 3)
            {
                ScreenManager.SpriteBatch.DrawString(ScreenManager.Font,
                    "3D Mode coming soon! Please enjoy 2 dimensions for now.",
                    new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, ScreenManager.GraphicsDevice.Viewport.Height / 2),
                    new Color(1.0f, 0.0f, 0.0f, 0.2f));
            } */

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
                MessageBoxScreen messageBox = new MessageBoxScreen("Would you like to quit?");
                messageBox.Accepted += QuitMessageBoxAccepted;
                ScreenManager.AddScreen(messageBox, ControllingPlayer);
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

            if (input.IsSwapRight(ControllingPlayer))
            {
                board.SwapRight();
                board.SwapLeft();
            }

            if (input.IsSwapUp(ControllingPlayer))
            {
                board.SwapUp();
            }

            if (input.IsSwapDown(ControllingPlayer))
            {
                board.SwapDown();
            }

            if (input.IsTurnLeft(ControllingPlayer))
            {
                camera.TurnBoard(Camera.ShiftState.Left);
            }
            
            if(input.IsTurnRight(ControllingPlayer) || input.IsTurnDefault(ControllingPlayer))
            {
                camera.TurnBoard(Camera.ShiftState.Right);
            }

            if (input.IsTurnUp(ControllingPlayer))
            {
                camera.TurnBoard(Camera.ShiftState.Up);
            }

            if (input.IsTurnDown(ControllingPlayer))
            {
                camera.TurnBoard(Camera.ShiftState.Down);
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            board.Update(gameTime);
            camera.Update(gameTime);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        private void QuitMessageBoxAccepted(Object sender, EventArgs e)
        {
            ExitToMainMenu();
        }

        private void ExitToMainMenu()
        {
            ExitScreen();
            ScreenManager.AddScreen(new BackgroundScreen(), ControllingPlayer);
            ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
        }
    }
}
