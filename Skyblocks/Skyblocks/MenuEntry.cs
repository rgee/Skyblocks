using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skyblocks
{
    /// <summary>
    /// Represents a single menu item in a menu. 
    /// </summary>
    public class MenuEntry
    {
        private string text;
        /// <summary>
        /// The text string for this menu entry.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private float selectionFade;

        /// <summary>
        /// Event that is raised when a menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;

        /// <summary>
        /// Handler for the selected event.
        /// </summary>
        /// <param name="playerIndex"></param>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
            {
                Selected(this, new PlayerIndexEventArgs(playerIndex));
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text"></param>
        public MenuEntry(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// Update this menu entry.
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="isSelected"></param>
        /// <param name="gameTime"></param>
        public virtual void Update(MenuScreen screen, bool isSelected,
                                                      GameTime gameTime)
        {
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
            {
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            }
            else
            {
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
            }
        }

        /// <summary>
        /// Draw the menu item.
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="position"></param>
        /// <param name="isSelected"></param>
        /// <param name="gameTime"></param>
        public virtual void Draw(MenuScreen screen, Vector2 position,
                                 bool isSelected, GameTime gameTime)
        {
            Color color = isSelected ? Color.Yellow : Color.White;

            double time = gameTime.TotalGameTime.TotalSeconds;

            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1 + pulsate * 0.05f * selectionFade;

            // Fade the text during transitions
            color = new Color(color.R, color.G, color.B, screen.TransitionAlpha);

            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 origin = new Vector2(140, font.LineSpacing / 2);

            spriteBatch.DrawString(font, text, position, color, 0,
                                    origin, scale, SpriteEffects.None, 0);
            screen.ScreenManager.GraphicsDevice.RenderState.DepthBufferEnable = true;
            screen.ScreenManager.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            screen.ScreenManager.GraphicsDevice.RenderState.AlphaTestEnable = false;
        }

        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.ScreenManager.Font.LineSpacing;
        }
    }
}
