using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Geometry;
using JigLibX.Physics;
using JigLibX.Collision;

namespace Sky
{
    /// <summary>
    /// Defines a single block in the large cube of blocks.
    /// We define this to be a separate class because it will
    /// have special picking code for user interaction.
    /// </summary>
    public class Block : DrawableGameComponent
    {
        private Vector3 position;
        private Vector3 scale;

        private Body body;
        public Body Body
        {
            get { return body; }
        }

        private CollisionSkin skin;
        public CollisionSkin Skin
        {
            get { return skin; }
        }

        public Block(Game game, Vector3 position, Vector3 scale)
            :base(game)
        {
            this.position = position;
            this.scale = scale;

            body = new Body();
            skin = new CollisionSkin(body);
            body.CollisionSkin = skin;

            Box box = new Box(Vector3.Zero, Matrix.Identity, scale);
            //skin.AddPrimitive(box, (int)MaterialTable.MaterialID.UserDefined, new MaterialProperties(0.8f, 0.8f, 0.7f));
        }
        
    }
}
