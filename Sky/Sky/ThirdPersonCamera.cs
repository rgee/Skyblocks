using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sky
{
    /// <summary>
    /// A third-person camera class. Takes a player object to track.
    /// Looks at a point fwdDistance units in front of the player.
    /// </summary>
    class ThirdPersonCamera : Camera
    {
        private Player target;
        private Vector3 offset = new Vector3(0f, 150f, -600f);
        private int prevScrollWheelVal;

        private const float fwdDistance = 50f;

        private float zoom = -200f;
        private const float altitude = 150f;

        
        public ThirdPersonCamera(float viewAngle, Viewport viewPort, float nearPlaneDist, float farPlaneDist, Player avatar)
            : base(viewAngle, viewPort, nearPlaneDist, farPlaneDist)
        {
            target = avatar;
            this.position.Y = altitude;
        }

        public override void Init(Vector3 startPosition, float hRotation, float vRotation)
        {
            
        }

        public override void AddToPosition(Vector3 toAdd)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            // Camera transformation is now completely dependant upon the player's position.
            // Working in cylindrical coordinates until I figure out how to simulate cylindrical
            // using spherical.

            Vector3 camLookAt = target.Position;
            System.Console.WriteLine(target.Position.ToString());
            camLookAt.X += fwdDistance * (float)Math.Cos(target.Yaw * MathHelper.Pi / 180f);
            camLookAt.Z += fwdDistance * (float)Math.Sin(target.Yaw * MathHelper.Pi / 180f);


            this.position.X = camLookAt.X - (float)Math.Cos(target.Yaw * MathHelper.Pi / 180f) * (float)Math.Cos(-.5f) * zoom;
            this.position.Y = camLookAt.Y + (float)Math.Sin(-.5f) * zoom;
            this.position.Z = camLookAt.Z - (float)Math.Sin(target.Yaw * MathHelper.Pi / 180f) * (float)Math.Cos(-.5f) * zoom;

            MouseState mouseState = Mouse.GetState();
            if (mouseState.ScrollWheelValue <= prevScrollWheelVal)
                zoom += 50;
                
            if (mouseState.ScrollWheelValue >= prevScrollWheelVal)
                zoom -= 50;

            

            prevScrollWheelVal = mouseState.ScrollWheelValue;

            /*
            this.position.X = camLookAt.X - (float)Math.Cos(target.Yaw * MathHelper.PiOver2) * zoom;
            this.position.Z = camLookAt.Z - (float)Math.Sin(target.Yaw * MathHelper.PiOver2) * zoom;
            */


            this.view = Matrix.CreateLookAt(this.position, camLookAt, new Vector3(0.0f, 1.0f, 0.0f));

        }
    }
}
