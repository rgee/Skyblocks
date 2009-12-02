using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Skyblocks
{
    public class MainMenuScreen : MenuScreen
    {
        public MainMenuScreen()
            : base("Main Menu")
        {

            MenuEntry play2DGameMenuEntry = new MenuEntry("Play Game (2D)");
            MenuEntry play3DGameMenuEntry = new MenuEntry("Play Game (3D)");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");
            

            // Hook event handlers
            play2DGameMenuEntry.Selected += PlayGame2DMenuEntrySelected;
            play3DGameMenuEntry.Selected += PlayGame3DMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(play2DGameMenuEntry);
            MenuEntries.Add(play3DGameMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        /// <summary>
        /// Event handler for when the 2D play game option is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PlayGame2DMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();
            ScreenManager.AddScreen(new GameplayScreen(2), null);
        }

        /// <summary>
        /// Event handler for when the 3D play game option is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PlayGame3DMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            MessageBoxScreen dimensionWarningBox = new MessageBoxScreen("Sorry! 3D Mode coming soon!");
            dimensionWarningBox.Accepted += WarningBoxAccepted;
            ScreenManager.AddScreen(dimensionWarningBox, null);
            
            /*
            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();
            ScreenManager.AddScreen(new GameplayScreen(3), null);
             */
        }

        /// <summary>
        /// Since we know a cancel from this screen means exit the game,
        /// exit the game after all screens transition out.
        /// </summary>
        /// <param name="playerIndex"></param>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            base.OnCancel(playerIndex);
            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();
        }

        void WarningBoxAccepted(object sender, EventArgs e)
        {
            ScreenManager.GetScreens().Last().ExitScreen();
        }
    }
}
