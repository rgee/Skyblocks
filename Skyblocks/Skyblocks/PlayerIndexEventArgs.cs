
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Skyblocks
{
    /// <summary>
    /// Custom event consisting of the index of the player.
    /// </summary>
    public class PlayerIndexEventArgs : EventArgs
    {
        private PlayerIndex playerIndex;
        /// <summary>
        /// The index of the player that triggered the event.
        /// </summary>
        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
        }

        public PlayerIndexEventArgs(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;
        }
    }
}
