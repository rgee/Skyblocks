using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;

namespace Sky
{
    /// <summary>
    /// Base class for any objects that will be affected by the physics system.
    /// </summary>
    public abstract class PhysicsEntity : DrawableGameComponent
    {
        protected Body body;
        protected CollisionSkin skin;

        protected Model model;
        protected Effect effect;

        protected Vector3 scale = Vector3.One;


        Matrix[] boneTransforms = null;
        int boneCount = 0;


        public Body Body
        {
            get { return body; }
        }
        public CollisionSkin Skin
        {
            get { return skin; }
        }

        public PhysicsEntity(Game game, Model model)
            : base(game)
        {
            this.model = model;
        }

        /// <summary>
        /// Set the mass of this physics entity.
        /// Source: JigLibX samples
        /// </summary>
        /// <param name="mass"></param>
        /// <returns></returns>
        protected Vector3 SetMass(float mass)
        {
            PrimitiveProperties primitiveProperties =
                new PrimitiveProperties(PrimitiveProperties.MassDistributionEnum.Solid,
                                        PrimitiveProperties.MassTypeEnum.Density,
                                        mass);
            float junk;
            Vector3 com;
            Matrix it;
            Matrix itCoM;

            skin.GetMassProperties(primitiveProperties, out junk, out com, out it, out itCoM);
            body.BodyInertia = itCoM;
            body.Mass = junk;

            return com;
        }

        public override void Draw(GameTime gameTime)
        {
            if (boneTransforms == null || boneCount != model.Bones.Count)
            {
                boneTransforms = new Matrix[model.Bones.Count];
                boneCount = model.Bones.Count;
            }

            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            FirstPersonCamera camera = ((Game1)this.Game).Camera;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index] *
                                   Matrix.CreateScale(scale) *
                                   body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation *
                                   body.Orientation *
                                   Matrix.CreateTranslation(body.Position);
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }
        }
    }
}
