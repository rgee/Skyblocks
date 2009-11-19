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

            MenuEntry playGameMenuEntry = new MenuEntry("Play Game");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");
            

            // Hook event handlers
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        /// <summary>
        /// Event handler for when the play game option is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();
            ScreenManager.AddScreen(new GameplayScreen(), null);
        }        
    }
}
