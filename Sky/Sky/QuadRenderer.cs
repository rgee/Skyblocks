using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sky
{
    /// <summary>
    /// Quad Renderer originally written and published at www.ziggyware.com
    /// </summary>
    class QuadRenderer : DrawableGameComponent
    {
        private VertexDeclaration vertexDecl = null;
        private VertexPositionTexture[] vertices = null;
        private short[] indices = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The game object</param>
        public QuadRenderer(Game game)
            : base(game)
        {
            IGraphicsDeviceService graphicsService =
                (IGraphicsDeviceService)base.Game.Services.GetService(typeof(IGraphicsDeviceService));
            vertexDecl = new VertexDeclaration(graphicsService.GraphicsDevice, VertexPositionTexture.VertexElements);
            
            // Define the quad
            vertices = new VertexPositionTexture[]
                        {
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(1,1)),
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(0,1)),
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(0,0)),
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(1,0))
                        };

            indices = new short[] { 0, 1, 2, 2, 3, 0 };

        }

        /// <summary>
        /// Render to the quad
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        public void Render(Vector2 vec1, Vector2 vec2)
        {
            IGraphicsDeviceService graphicsService =
                        (IGraphicsDeviceService)base.Game.Services.GetService(typeof(IGraphicsDeviceService));

            GraphicsDevice device = graphicsService.GraphicsDevice;
            device.VertexDeclaration = vertexDecl;

            vertices[0].Position.X = vec2.X;
            vertices[0].Position.Y = vec1.Y;

            vertices[1].Position.X = vec1.X;
            vertices[1].Position.Y = vec1.Y;

            vertices[2].Position.X = vec1.X;
            vertices[2].Position.Y = vec2.Y;

            vertices[3].Position.X = vec2.X;
            vertices[3].Position.Y = vec2.Y;

            device.DrawIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, vertices, 0, 4, indices, 0, 2);
        }
    }
}
