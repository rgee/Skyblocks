using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Skyblocks
{
    /// <summary>
    /// A message box screen used for confirmations
    /// </summary>
    class MessageBoxScreen : GameScreen
    {
        string message;

        /// <summary>
        /// The event handler for when the accept option is selected
        /// </summary>
        public event EventHandler<EventArgs> Accepted;

        /// <summary>
        /// The event handler for when the cancel option is selected
        /// </summary>
        public event EventHandler<EventArgs> Cancelled;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        public MessageBoxScreen(string message)
            : this(message, true)
        { }

        public MessageBoxScreen(string message, bool includeUsageText)
        {
            const string usage = "\nEnter, Space = ok" + "\nEsc = cancel";

            if (includeUsageText)
                this.message = message + usage;
            else
                this.message = message;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.3);
            TransitionOffTime = TimeSpan.FromSeconds(0.3);
        }

        /// <summary>
        /// Handles keyboard events and fires accept/cancel events
        /// </summary>
        /// <param name="input"></param>
        public override void HandleInput(InputState input)
        {
            if (input.IsMenuSelect(ControllingPlayer))
            {
                if (Accepted != null)
                    Accepted(this, EventArgs.Empty);
                ExitScreen();
            }
            else if (input.IsMenuCancel(ControllingPlayer))
            {
                if (Cancelled != null)
                    Cancelled(this, EventArgs.Empty);
                ExitScreen();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            ScreenManager.FadeToBlack(TransitionAlpha * 2 / 3);

            Viewport viewPort = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewPort.Width, viewPort.Height);
            Vector2 textSize = font.MeasureString(message);
            Vector2 textPos = (viewportSize - textSize) / 2;

            Color color = new Color(255, 255, 255, TransitionAlpha);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, message, textPos, color);

            // Reset render states to make sure 3D rendering still works after this
            // draw call.
            ScreenManager.GraphicsDevice.RenderState.DepthBufferEnable = true;
            ScreenManager.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            ScreenManager.GraphicsDevice.RenderState.AlphaTestEnable = false;

            spriteBatch.End();
        }
    }
}
