using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skyblocks
{
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    /// <summary>
    /// The base class for all screens. A screen encapsulates update and draw
    /// logic. All screens are inherited from GameScreen.
    /// </summary>
    public abstract class GameScreen
    {
        private bool isPopup = false;
        /// <summary>
        /// Is this screen a popup?
        /// </summary>
        public bool IsPopup
        {
            get { return isPopup; }
            protected set { isPopup = value; }
        }


        private TimeSpan transitionOnTime;
        /// <summary>
        /// The time it takes for a screen to be transitioned on once it is
        /// activated.
        /// </summary>
        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        private TimeSpan transitionOffTime;
        /// <summary>
        /// The time it takes for a screen to be transitioned off once it is
        /// deactivated.
        /// </summary>
        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }


        private PlayerIndex? controllingPlayer;
        /// <summary>
        /// Get the current player controlling this screen. Used to limit
        /// input handling to only the player with control if necessary.
        /// </summary>
        public PlayerIndex? ControllingPlayer
        {
            get { return controllingPlayer; }
            internal set { controllingPlayer = value; }
        }

        private float transitionPosition = 1;
        /// <summary>
        /// The current stage of this screen's position. 0 being fully off
        /// the screen and 1 being fully active.
        /// </summary>
        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }
        
        /// <summary>
        /// Get the alpha value for this screen's current transition position.
        /// Allows for fading in and out.
        /// </summary>
        public byte TransitionAlpha
        {
            get { return (byte)(255 - transitionPosition * 255); }
        }

        private ScreenState screenState = ScreenState.TransitionOn;
        /// <summary>
        /// Get the current screen state. See the ScreenState enum for
        /// possible values.
        /// </summary>
        public ScreenState ScreenState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }

        private bool isExiting = false;
        /// <summary>
        /// Is the screen exiting to make room for another or exiting
        /// for good?
        /// </summary>
        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }

        /// <summary>
        /// Is this screen active? We consider input during a screen's transition.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return !otherScreenHasFocus && (screenState == ScreenState.Active ||
                                                screenState == ScreenState.TransitionOn);
            }
        }

        private bool otherScreenHasFocus;

        private ScreenManager screenManager;
        /// <summary>
        /// Get the screen manager associated with this screen.
        /// </summary>
        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            internal set { screenManager = value; }
        }

        /// <summary>
        /// All content loading occurs here.
        /// </summary>
        public virtual void LoadContent() { }


        public virtual void UnloadContent() { }

        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                      bool coveredByOtherScreen)
        {
            this.otherScreenHasFocus = otherScreenHasFocus;

            if (isExiting)
            {
                // Mark the screen as transitioning off
                screenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    ScreenManager.RemoveScreen(this);
                }
            }
            else if (coveredByOtherScreen)
            {
                // Screens transition off if they are covered by another.
                if (UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    screenState = ScreenState.TransitionOff;
                }
                else
                {
                    screenState = ScreenState.Hidden;
                }
            }
            else
            {
                // If the screen isn't leaving and it isn't covered by something else
                // then it should transition onscreen.
                if (UpdateTransition(gameTime, transitionOnTime, -1))
                {
                    screenState = ScreenState.TransitionOn;
                }
                else
                {
                    screenState = ScreenState.Active;
                }
            }
        }

        /// <summary>
        /// Helper method for updating the screen's position during transitions
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="time"></param>
        /// <param name="direction"></param>
        /// <returns>True if the screen is still transitioning, false otherwise.</returns>
        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            transitionPosition += transitionDelta * direction;

            if (((direction < 0) && (transitionPosition <= 0)) ||
                ((direction > 0) && (transitionPosition >= 1)))
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }

            return true;
        }

        /// <summary>
        /// This method is only called when the screen is active and allows
        /// the screen to handle user input.
        /// </summary>
        /// <param name="input"></param>
        public virtual void HandleInput(InputState input) { }

        public virtual void Draw(GameTime gameTime) { }

        /// <summary>
        /// Gets rid of the screen and allows it to transition off.
        /// </summary>
        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
            {
                ScreenManager.RemoveScreen(this);
            }
            else
            {
                isExiting = true;
            }
        }
    }
}
