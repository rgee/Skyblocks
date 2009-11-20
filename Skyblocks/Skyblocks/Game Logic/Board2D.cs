using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
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
        /// <summary>
        /// An array that holds the two blocks that are currently shifting.
        /// Logically, no more than two blocks should be shifting at a time
        /// so we keep them here to perform special operations on them easily.
        /// </summary>
        private List<Block> shiftingBlocks = new List<Block>();

        private int width;
        /// <summary>
        /// How many cubes wide is the board?
        /// </summary>
        public int Width
        {
            get { return width; }
            set { width = value; }
        }


        private int selectedBlockX = 5;
        private int selectedBlockY = 5;


        private float blockSize;

        /// <summary>
        /// The X-coordinate (In board space.) of the selected block.
        /// </summary>
        public int SelectedBlockX
        {
            get { return selectedBlockX; }
            set { selectedBlockX = value; }
        }

        /// <summary>
        /// The Y-coordinate (In board space.) of the selected block.
        /// </summary>
        public int SelectedBlockY
        {
            get { return selectedBlockY; }
            set { selectedBlockY = value; } 
        }

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



        /// <summary>
        /// Is the board currently performing an animated shift?
        /// </summary>
        public bool IsShifting
        {
            get { return shiftingBlocks.Count > 0; }
        }

        private const float shiftDuration = 0.1f;
        float currentShiftTime;


        public Board2D(int width, int height, GameplayScreen screen)
        {
            Debug.Assert((width <= 10) && (height <= 10));

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

            layout[selectedBlockX, selectedBlockY].IsSelected = true;
        }

        public void Update(GameTime gameTime)
        {

            if (IsShifting)
            {
                currentShiftTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (currentShiftTime > shiftDuration)
                {
                    boardPositionMatrices[shiftingBlocks[0].XLayoutPosition, shiftingBlocks[0].YLayoutPosition] = shiftingBlocks[0].World;
                    boardPositionMatrices[shiftingBlocks[1].XLayoutPosition, shiftingBlocks[1].YLayoutPosition] = shiftingBlocks[1].World;

                    shiftingBlocks.Clear();
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
            blockSize = blocks[0].Model.Meshes[0].BoundingSphere.Radius * sphereScale * 1.9f;

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

        /// <summary>
        /// Draw each block.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
            foreach (Block block in blocks)
            {
                if (!shiftingBlocks.Contains(block))
                {
                    block.Draw(screen.Camera, gameTime);
                }
            }

            if (shiftingBlocks.Count > 0)
            {
                Matrix toTransform, fromTransform;

                // Linearly interpolate between the world matrcies of the to and from blocks
                // to eventually end up with them both in the other's original positioning.
                Matrix.Lerp(ref boardPositionMatrices[shiftingBlocks[0].XLayoutPosition,
                                                      shiftingBlocks[0].YLayoutPosition],
                            ref boardPositionMatrices[shiftingBlocks[1].XLayoutPosition,
                                                      shiftingBlocks[1].YLayoutPosition],
                            currentShiftTime / shiftDuration,
                            out fromTransform);
                
                Matrix.Lerp(ref boardPositionMatrices[shiftingBlocks[1].XLayoutPosition,
                                                      shiftingBlocks[1].YLayoutPosition],
                            ref boardPositionMatrices[shiftingBlocks[0].XLayoutPosition,
                                                      shiftingBlocks[0].YLayoutPosition],
                            currentShiftTime / shiftDuration,
                            out toTransform); 

               // boardPositionMatrices[shiftingBlocks[0].XLayoutPosition, shiftingBlocks[0].YLayoutPosition] = fromTransform;
               // boardPositionMatrices[shiftingBlocks[1].XLayoutPosition, shiftingBlocks[1].YLayoutPosition] = toTransform;

                shiftingBlocks[0].World = fromTransform;
                shiftingBlocks[1].World = toTransform;


                shiftingBlocks[0].Draw(screen.Camera, gameTime);
                shiftingBlocks[1].Draw(screen.Camera, gameTime);
            }

        }

        /// <summary>
        /// Swap the current selected block with the one to its left.
        /// </summary>
        public void SwapLeft()
        {
            // Don't shift if the board is currently shifting.
            if (IsShifting) return;

            // Don't shift if the selected block is on the leftmost edge.
            if (selectedBlockX == 0) return;

            // Don't shift if there are no blocks to the left of the selected
            // block.
            if (!layout[selectedBlockX - 1, selectedBlockY].IsActive) return;

            // Tell the board to start shifting the selected blocks.
            shiftingBlocks.Add(layout[selectedBlockX, selectedBlockY]);
            shiftingBlocks.Add(layout[selectedBlockX - 1, selectedBlockY]);

            // Swap positions in the layout array.
            layout[selectedBlockX, selectedBlockY] = shiftingBlocks[1];
            layout[selectedBlockX - 1, selectedBlockY] = shiftingBlocks[0];

        }

        /// <summary>
        /// Select the block to the left of the current one.
        /// </summary>
        public void SelectLeft()
        {
            if (selectedBlockX == 0) return;
            if (!layout[selectedBlockX - 1, selectedBlockY].IsActive) return;

            layout[selectedBlockX, selectedBlockY].IsSelected = false;
            selectedBlockX--;
            layout[selectedBlockX, selectedBlockY].IsSelected = true;
        }

        /// <summary>
        /// Select the block to the right of the current one.
        /// </summary>
        public void SelectRight()
        {
            if (selectedBlockX == width - 1) return;
            if (!layout[selectedBlockX + 1, selectedBlockY].IsActive) return;

            layout[selectedBlockX, selectedBlockY].IsSelected = false;
            selectedBlockX++;
            layout[selectedBlockX, selectedBlockY].IsSelected = true;
        }

        /// <summary>
        /// Select the block below the current one.
        /// </summary>
        public void SelectDown()
        {
            if (selectedBlockY == 0) return;
            if (!layout[selectedBlockX, selectedBlockY - 1].IsActive) return;

            layout[selectedBlockX, selectedBlockY].IsSelected = false;
            selectedBlockY--;
            layout[selectedBlockX, selectedBlockY].IsSelected = true;
        }

        /// <summary>
        /// Select the block above the current one.
        /// </summary>
        public void SelectUp()
        {
            if (selectedBlockY == height - 1) return;
            if (!layout[selectedBlockX, selectedBlockY + 1].IsActive) return;

            layout[selectedBlockX, selectedBlockY].IsSelected = false;
            selectedBlockY++;
            layout[selectedBlockX, selectedBlockY].IsSelected = true;
        }
    }
}
