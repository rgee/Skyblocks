using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Skyblocks
{
    /// <summary>
    /// A screen that displays the background for a menu. Does not transition on or off.
    /// </summary>
    public class BackgroundScreen : GameScreen
    {
        ContentManager content;
        Texture2D backgroundTexture;

        public BackgroundScreen()
        {
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            if (content == null)
            {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            }

            backgroundTexture = content.Load<Texture2D>("Menus//Menu");
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewPort = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewPort.Width, viewPort.Height);
            byte fade = TransitionAlpha;
            spriteBatch.Begin(SpriteBlendMode.None);
            spriteBatch.Draw(backgroundTexture, fullscreen, new Color(fade, fade, fade));

            ScreenManager.GraphicsDevice.RenderState.DepthBufferEnable = true;
            ScreenManager.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            ScreenManager.GraphicsDevice.RenderState.AlphaTestEnable = false;
            spriteBatch.End();
        }
    }
}
