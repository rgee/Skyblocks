using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Collision;
using JigLibX.Math;

namespace Sky.PhysicsEntities
{
    public class PlaneEntity : PhysicsEntity
    {
        public PlaneEntity(Game game, Model model, float d)
            : base(game, model)
        {
            body = new Body();
            skin = new CollisionSkin(null);
            body.CollisionSkin = skin;
            skin.AddPrimitive(new JigLibX.Geometry.Plane(Vector3.Up, d), new MaterialProperties(0.2f, 0.7f, 0.6f));

            body.MoveTo(new Vector3(0, 15, 0), Matrix.Identity);
            skin.ApplyLocalTransform(new Transform(new Vector3(0, 15,0), Matrix.Identity));

            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(skin);
        }

        public override void Draw(GameTime gameTime)
        {
            this.Game.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            this.Game.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

            base.Draw(gameTime);
        }
    }
}
