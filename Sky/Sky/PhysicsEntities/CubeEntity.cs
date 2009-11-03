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
    public class CubeEntity : PhysicsEntity
    {
        public CubeEntity(Game game, Model model, Vector3 sideLengths, Matrix orientation, Vector3 position)
            : base(game, model)
        {
            body = new Body();
            skin = new CollisionSkin(body);

            skin.AddPrimitive(new Box(-0.5f * sideLengths, orientation, sideLengths), new MaterialProperties(0.5f, 0.2f, 0.2f));
            body.CollisionSkin = this.skin;
            Vector3 com = SetMass(1.0f);
            body.MoveTo(position, Matrix.Identity);
            skin.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
            body.EnableBody();
            this.scale = sideLengths;
            body.Force = new Vector3(0, -200, 0);
        }
    }
}
