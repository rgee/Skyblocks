using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Skyblocks
{
    /// <summary>
    /// Represents a (currently) 2D board on the screen.
    /// </summary>
    public class Board2D
    {
        private int width;
        /// <summary>
        /// How many cubes wide is the board?
        /// </summary>
        public int Width
        {
            get { return width; }
            set { width = value; }
        }


        private float pieceSpacing;

        /// <summary>
        /// The screen this board is associated with. 
        /// </summary>
        private GameplayScreen screen;

        private int height;
        /// <summary>
        /// How many cubes high is the board?
        /// </summary>
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// 2D array of Piece representing the layout of the board. Used for
        /// game logic concerning the relative position of pieces on the board.
        /// </summary>
        private Block[,] layout;

        private Matrix[] pieceTransforms;

        /// <summary>
        /// We keep a sequential copy of all of the pieces for fast iteration.
        /// </summary>
        private List<Block> blocks = new List<Block>();
        
        /// <summary>
        /// A collection of blocks on the board.
        /// </summary>
        public IEnumerable<Block> Pieces
        {
            get { return blocks; }
        }

        /// <summary>
        /// 2D array of Matrices representing the position transforms
        /// to align each Piece into a grid.
        /// </summary>
        private Matrix[,] boardPositionMatrices;
        
        public Board2D(int width, int height, GameplayScreen screen)
        {
            this.width = width;
            this.height = height;
            this.screen = screen;

            this.layout = new Block[width, height];
            this.boardPositionMatrices = new Matrix[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Block block = new Block();
                    block.XLayoutPosition = x;
                    block.YLayoutPosition = y;
                    layout[x, y] = block;

                    blocks.Add(block);
                }
            }
        }

        public void LoadContent()
        {
            foreach (Block block in blocks)
                block.LoadContent(screen.Content);

            pieceTransforms = new Matrix[blocks[0].Model.Bones.Count];
            blocks[0].Model.CopyAbsoluteBoneTransformsTo(pieceTransforms);

            float sphereScale = Math.Max(pieceTransforms[0].M11, pieceTransforms[0].M22);
            float blockSize = blocks[0].Model.Meshes[0].BoundingSphere.Radius * sphereScale;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    

                    Vector3 blockPos = new Vector3(
                        (Width - 1) * -0.5f, (Height - 1) * -0.5f, 0.0f);
                    blockPos += new Vector3(x, y, 0.0f);
                    blockPos *= blockSize;

                    Matrix transform = Matrix.CreateTranslation(blockPos);

                    boardPositionMatrices[x, y] = transform;
                    layout[x, y].World = transform;
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (Block block in blocks)
            {
                block.Draw(screen.Camera, gameTime);
            }
        }
    }
}
