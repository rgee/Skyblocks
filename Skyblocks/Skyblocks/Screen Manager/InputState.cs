using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Skyblocks
{
    /// <summary>
    /// Encapsulates input device polling and allows translation
    /// to higher level key commands. (i.e. Using a method such as
    /// IsJump() instead of checking directly if the space key is
    /// pressed.)
    /// </summary>
    public class InputState
    {
        /// <summary>
        /// Maximum inputs to keep track of. One per player.
        /// </summary>
        public const int MAX_INPUTS = 4;

        public readonly KeyboardState[] CurrentKeyboardStates;

        public readonly KeyboardState[] PreviousKeyboardStates;

        public InputState()
        {
            CurrentKeyboardStates = new KeyboardState[MAX_INPUTS];
            PreviousKeyboardStates = new KeyboardState[MAX_INPUTS];
        }

        /// <summary>
        /// Retrieves the latest keyboard states.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < MAX_INPUTS; i++)
            {
                PreviousKeyboardStates[i] = CurrentKeyboardStates[i];

                CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
            }
        }

        /// <summary>
        /// Check if a key was pressed during this update.
        /// </summary>
        /// <param name="key">The key to check for.</param>
        /// <param name="controllingPlayer">The player in control. If null, will check for any player's input.</param>
        /// <param name="playerIndex">The player index of the player who pressed the key.</param>
        /// <returns></returns>
        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer,
                                            out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                playerIndex = controllingPlayer.Value;
                int i = (int)playerIndex;
                return (CurrentKeyboardStates[i].IsKeyDown(key) &&
                        PreviousKeyboardStates[i].IsKeyUp(key));
            }
            else
            {
                // Accept input from any player.
                return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
            }
        }

        /// <summary>
        /// Check for the "menu select" input action.
        /// </summary>
        /// <param name="controllingPlayer">The player to check input for. If null, will check for any player's input.</param>
        /// <param name="playerIndex">The index of the player who caused the menu select action.</param>
        /// <returns></returns>
        public bool IsMenuSelect(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
                   IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Check for the "menu cancel" input action.
        /// </summary>
        /// <param name="controllingPlayer">The player to check input for. If null, will check for any player's input.</param>
        /// <param name="playerIndex">The index of the player who caused the menu cancel action.</param>
        /// <returns></returns>
        public bool IsMenuCancel(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Check for the "menu up" input action.
        /// </summary>
        /// <param name="controllingPlayer">The player to check input for. If null, will check for any player's input.</param>
        /// <param name="playerIndex">The index of the player who caused the menu up action.</param>
        /// <returns></returns>
        public bool IsMenuUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Check for the "menu down" input action.
        /// </summary>
        /// <param name="controllingPlayer">The player to check input for. If null, will check for any player's input.</param>
        /// <returns></returns>
        public bool IsMenuDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Check for the "pause game" input action.
        /// </summary>
        /// <param name="controllingPlayer">The player to check input for. If null, will check for any player's input.</param>
        /// <param name="playerIndex">The index of the player who caused the pause game action.</param>
        /// <returns></returns>
        public bool IsPauseGame(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Allows the gameplay screen to determine if a player selects a block to the left.
        /// </summary>
        /// <param name="controllingPlayer"></param>
        /// <returns></returns>
        public bool IsSelectLeft(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.Left, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Allows the gameplay screen to determine if a player selects a block to the right.
        /// </summary>
        /// <param name="controllingPlayer"></param>
        /// <returns></returns>
        public bool IsSelectRight(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.Right, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Allows the gameplay screen to determine if a player selects a block below the current one.
        /// </summary>
        /// <param name="controllingPlayer"></param>
        /// <returns></returns>
        public bool IsSelectDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Allows the gameplay screen to determine if a player selects a block above the current one.
        /// </summary>
        /// <param name="controllingPlayer"></param>
        /// <returns></returns>
        public bool IsSelectUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Swap the selected block with the block to its left.
        /// </summary>
        /// <param name="controllingPlayer"></param>
        /// <returns></returns>
        public bool IsSwapLeft(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.A, controllingPlayer, out playerIndex) &&
                !IsNewKeyPress(Keys.LeftControl, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Swap the selected block with the block to its right.
        /// </summary>
        /// <param name="controllingPlayer"></param>
        /// <returns></returns>
        public bool IsSwapRight(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.D, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Swap the selected block with the block above it.
        /// </summary>
        /// <param name="controllingPlayer"></param>
        /// <returns></returns>
        public bool IsSwapUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.W, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Swap the selected block with the block below it.
        /// </summary>
        /// <param name="controllingPlayer"></param>
        /// <returns></returns>
        public bool IsSwapDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.S, controllingPlayer, out playerIndex);
        }

        public bool IsTurnLeft(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.NumPad4, controllingPlayer, out playerIndex);
        }

        public bool IsTurnRight(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.NumPad6, controllingPlayer, out playerIndex);
        }

        public bool IsTurnUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.NumPad8, controllingPlayer, out playerIndex);
        }

        public bool IsTurnDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.NumPad2, controllingPlayer, out playerIndex);
        }
    }
}
