using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace Skyblocks
{
    public class ScreenManager : DrawableGameComponent
    {
        /// <summary>
        /// The game screens this screen manager manages.
        /// </summary>
        private List<GameScreen> screens = new List<GameScreen>();

        /// <summary>
        /// The game screens marked for updating.
        /// </summary>
        private List<GameScreen> screensToUpdate = new List<GameScreen>();

        /// <summary>
        /// The input state manager.
        /// </summary>
        private InputState input = new InputState();

        private SpriteBatch spriteBatch;
        /// <summary>
        /// The sprite batch to share between screens.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        private SpriteFont font;
        /// <summary>
        /// The sprite font to share between screeens.
        /// </summary>
        public SpriteFont Font
        {
            get { return font; }
        }

        /// <summary>
        /// A blank texture. Useful for transition fades.
        /// </summary>
        private Texture2D blankTexture;

        private bool isInitialized;

        private bool traceEnabled = false;
        /// <summary>
        /// If tracing is enabled, the collection of screens is printed on each
        /// update. Debugging tool.
        /// </summary>
        public bool TraceEnabled
        {
            get { return traceEnabled; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="game"></param>
        public ScreenManager(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Initializer.
        /// </summary>
        public override void Initialize()
        {
            isInitialized = true;
            base.Initialize();
        }

        /// <summary>
        /// Load all assets.
        /// </summary>
        protected override void LoadContent()
        {
            ContentManager content = Game.Content;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = content.Load<SpriteFont>("menufont");
            blankTexture = content.Load<Texture2D>("blank");

            // For levels and such, screens with such functionality
            // should manage this in their update, draw and content loading
            // logic.
            //
            // TODO: Add ability to load additional content on the fly, not
            // just on startup.
            foreach (GameScreen screen in screens)
            {
                screen.LoadContent();
            }

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (screens.Count == 0)
            {
                Game.Exit();
            }

            input.Update();

            // Reset the update list
            screensToUpdate.Clear();
            foreach (GameScreen screen in screens)
            {
                screensToUpdate.Add(screen);
            }

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            while (screensToUpdate.Count != 0)
            {
                GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];
                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                   screen.ScreenState == ScreenState.Active)
                {
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(input);
                        otherScreenHasFocus = true;
                    }

                    if (!screen.IsPopup)
                    {
                        coveredByOtherScreen = true;
                    }
                }
            }

            if (traceEnabled)
                TraceScreens();

            base.Update(gameTime);
        }

        /// <summary>
        /// Print a list of the screens.
        /// </summary>
        private void TraceScreens()
        {
            List<string> screenNames = new List<string>();

            foreach (GameScreen screen in screens)
            {
                screenNames.Add(screen.GetType().ToString());
            }

            Trace.WriteLine(string.Join(", ", screenNames.ToArray()));
        }

        /// <summary>
        /// Draw any active screens.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in screens)
            {
                screen.Draw(gameTime);
            }
            
            base.Draw(gameTime);
        }

        /// <summary>
        /// Add a screen to the screen manager.
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="controllingPlayer"></param>
        public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
        {
            screen.ControllingPlayer = controllingPlayer;
            screen.ScreenManager = this;
            screen.IsExiting = false;

            if (isInitialized)
            {
                screen.LoadContent();
            }

            screens.Add(screen);
        }

        /// <summary>
        /// INSTANTLY remove a game screen from the screen manager. NO TRANSITION.
        /// </summary>
        /// <param name="screen"></param>
        public void RemoveScreen(GameScreen screen)
        {
            screens.Remove(screen);
            screensToUpdate.Remove(screen);
        }

        /// <summary>
        /// Get a copy of the master screen list.
        /// </summary>
        /// <returns></returns>
        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }


        /// <summary>
        /// Draws a fullscreen, black, translucent sprite. Useful for
        /// fading to black between screens.
        /// </summary>
        /// <param name="alpha"></param>
        public void FadeToBlack(int alpha)
        {
            Viewport viewPort = GraphicsDevice.Viewport;

            spriteBatch.Begin();

            spriteBatch.Draw(blankTexture,
                             new Rectangle(0, 0, viewPort.Width, viewPort.Height),
                             new Color(0, 0, 0, (byte)alpha));
            spriteBatch.End();
        }
    }
}
