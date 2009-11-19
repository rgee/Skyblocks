using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Skyblocks
{
    /// <summary>
    /// Base class for a menu screen. Handles input and transitioning.
    /// Inherited classes can override methods to change appearance and content.
    /// </summary>
    public abstract class MenuScreen : GameScreen
    {
        private List<MenuEntry> menuEntries = new List<MenuEntry>();
        /// <summary>
        /// Gets the list of menu entries.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        /// <summary>
        /// The current index of the selected menu entry.
        /// </summary>
        private int selectedEntry;

        private string menuTitle;

        public MenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void HandleInput(InputState input)
        {
            if (input.IsMenuUp(ControllingPlayer))
            {
                selectedEntry--;

                if (selectedEntry < 0)
                {
                    selectedEntry = menuEntries.Count - 1;
                }
            }
            if (input.IsMenuDown(ControllingPlayer))
            {
                selectedEntry++;


                if (selectedEntry >= menuEntries.Count)
                {
                    Trace.WriteLine("Rolling over the bottom");
                    Trace.WriteLine(selectedEntry.ToString());
                    selectedEntry = 0;
                }
            }
            PlayerIndex playerIndex;
            if(input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                OnSelectEntry(selectedEntry, playerIndex);
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                OnCancel(playerIndex);
            }
        }

        /// <summary>
        /// If an entry is selected, let the MenuEntry know so it can fire events
        /// </summary>
        /// <param name="entryIndex"></param>
        /// <param name="playerIndex"></param>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[selectedEntry].OnSelectEntry(playerIndex);
        }

        /// <summary>
        /// If we need to cancel this menu, exit it.
        /// </summary>
        /// <param name="playerIndex"></param>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);
                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            Vector2 position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 200);

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
            {
                position.X -= transitionOffset * 256;
            }
            else
            {
                position.X += transitionOffset * 512;
            }

            spriteBatch.Begin();

            // Draw the menu entries
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, position, isSelected, gameTime);

                position.Y += menuEntry.GetHeight(this);
            }

            // Draw the menu title
            Vector2 titlePosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width /2, 80);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192, TransitionAlpha);
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            ScreenManager.GraphicsDevice.RenderState.DepthBufferEnable = true;
            ScreenManager.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            ScreenManager.GraphicsDevice.RenderState.AlphaTestEnable = false;

            spriteBatch.End();

        }
    }
}
