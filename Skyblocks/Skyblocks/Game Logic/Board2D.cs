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


        private int selectedBlockX = 2;
        private int selectedBlockY = 2;


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
        /// The possible colors of the blocks on this board.
        /// </summary>
        private List<Color> colors;

        /// <summary>
        /// RNG
        /// </summary>
        private Random random;

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
        /// The number of blocks in a row the player needs to 
        /// make a match.
        /// </summary>
        private const int matchAmount = 3;

        /// <summary>
        /// A flag to tell the board to scan for groups
        /// created during initialization and break them.
        /// </summary>
        private bool sanitizing = true;

        ContentManager content;

        /// <summary>
        /// Is the board currently performing an animated shift?
        /// </summary>
        public bool IsShifting
        {
            get { return shiftingBlocks.Count > 0; }
        }


        public Board2D(int width, int height, GameplayScreen screen)
        {
            Debug.Assert((width <= 10) && (height <= 10));

            random = new Random();
            colors = new List<Color>();
            colors.Add(Color.Red);
            colors.Add(Color.Green);
            colors.Add(Color.Purple);
            colors.Add(Color.Blue);
            colors.Add(Color.Yellow);
            colors.Add(Color.SeaShell);

            this.width = width;
            this.height = height;
            this.screen = screen;

            this.layout = new Block[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Block block = new Block();
                    block.XLayoutPosition = x;
                    block.YLayoutPosition = y;
                    layout[x, y] = block;

                    block.Color = colors[random.Next(colors.Count)];
                    blocks.Add(block);
                }
            }

            layout[selectedBlockX, selectedBlockY].IsSelected = true;

            // Analyze once to remove pre-made matches.
            for(int i = 0; i < 2; i++) AnalyzeBoard();
            sanitizing = false;
        }


        /// <summary>
        /// Checks the entire board for matches.
        /// </summary>
        /// <returns></returns>
        private bool AnalyzeBoard()
        {
            if (!IsShifting)
            {
                bool foundMatch = false;

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (layout[x, y].IsActive)
                        {
                            foundMatch |= AnalyzeGroup(x, y, 0, 1);
                            foundMatch |= AnalyzeGroup(x, y, 1, 0);
                        }
                    }
                }

                return foundMatch;
            }
            return false;
        }

        /// <summary>
        /// Scans for a match in the specified direction. Sanitizes the group
        /// if the board is set to sanitize.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        private bool AnalyzeGroup(int x, int y, int dx, int dy)
        {
            
            Color colorToMatch = layout[x, y].Color;
            int count = 0;
            List<Block> matchedBlocks = new List<Block>();

            for (int i = 0; i < matchAmount; i++, x += dx, y += dy)
            {
                if (x >= width) return false;
                if (y >= height) return false;

                matchedBlocks.Add(layout[x, y]);
                if(layout[x, y].Color == colorToMatch)
                    count++;
            }
            if (count == matchAmount)
            {
                if (sanitizing)
                {
                    // Break up the group by changing one random block to a new random color.
                    Color randomColor = matchedBlocks[0].Color;
                    while (randomColor == matchedBlocks[0].Color)
                    {
                        randomColor = colors[random.Next(colors.Count)];
                    }
                    matchedBlocks[random.Next(matchedBlocks.Count)].Color = randomColor;
                }
                else
                {
                    foreach (Block block in matchedBlocks)
                    {
                        // For now we just set the block to not draw.
                        // Animation to come.
                        block.IsActive = false;
                    }
                }
                return true;
            }
            return false;
        }

        public void Update(GameTime gameTime)
        {


            if (IsShifting)
            {
                shiftingBlocks[0].Update();
                shiftingBlocks[1].Update();


                if (shiftingBlocks[0].TransitionAmount == 1.0f && shiftingBlocks[1].TransitionAmount == 1.0f)
                {

                    shiftingBlocks.Clear();
                }
            }
            AnalyzeBoard();
        }



        public void LoadContent()
        {
            if (content == null)
            {
                content = new ContentManager(screen.ScreenManager.Game.Services, "Content");
            }


            foreach (Block block in blocks)
                block.LoadContent(content);

            pieceTransforms = new Matrix[blocks[0].Model.Bones.Count];
            blocks[0].Model.CopyAbsoluteBoneTransformsTo(pieceTransforms);

            // Get the size of each block from it's bounding sphere.
            float sphereScale = Math.Max(pieceTransforms[0].M11, pieceTransforms[0].M22);
            blockSize = blocks[0].Model.Meshes[0].BoundingSphere.Radius * sphereScale * 1.9f;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    // Align the grid

                    Vector3 blockPos = new Vector3(
                        (Width - 1) * -0.5f, (Height - 1) * -0.5f, 0.0f);
                    blockPos += new Vector3(x, y, 0.0f);
                    blockPos *= blockSize;

                    Matrix transform = Matrix.CreateTranslation(blockPos);
                    layout[x, y].Destination = layout[x, y].PrevLocation = transform;
                }
            }
        }

        public void UnloadContent()
        {
            content.Unload();
        }

        /// <summary>
        /// Draw each block.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
            foreach (Block block in blocks)
            {
                if (block.IsActive)
                {
                    block.Draw(screen.Camera, gameTime);
                }
            }

        }

        /// <summary>
        /// Swap the selected block with the block to its left.
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

            layout[selectedBlockX, selectedBlockY].IsSelected = false;
            layout[selectedBlockX - 1, selectedBlockY].IsSelected = true;

            // Swap positions in the layout array.
            layout[selectedBlockX, selectedBlockY] = shiftingBlocks[1];
            layout[selectedBlockX - 1, selectedBlockY] = shiftingBlocks[0];


            
            // Tell the board pieces to shift.
            Matrix block1World, block2World;
            block1World = shiftingBlocks[0].CurrentWorld;
            block2World = shiftingBlocks[1].CurrentWorld;

            shiftingBlocks[0].PrevLocation = block1World;
            shiftingBlocks[1].PrevLocation = block2World;

            shiftingBlocks[0].Destination = block2World;
            shiftingBlocks[1].Destination = block1World;

            shiftingBlocks[0].TransitionAmount = 0.0f;
            shiftingBlocks[1].TransitionAmount = 0.0f;

        }

        /// <summary>
        /// Swap the selected block with the block to its right.
        /// </summary>
        public void SwapRight()
        {
            // Don't shift if the board is currently shifting.
            if (IsShifting) return;

            // Don't shift if the selected block is on the edge.
            if (selectedBlockX == Width - 1) return;

            // Don't shift if there are no blocks to the left of the selected
            // block.
            if (!layout[selectedBlockX + 1, selectedBlockY].IsActive) return;

            // Tell the board to start shifting the selected blocks.
            shiftingBlocks.Add(layout[selectedBlockX, selectedBlockY]);
            shiftingBlocks.Add(layout[selectedBlockX + 1, selectedBlockY]);

            layout[selectedBlockX, selectedBlockY].IsSelected = false;
            layout[selectedBlockX + 1, selectedBlockY].IsSelected = true;

            // Swap positions in the layout array.
            layout[selectedBlockX, selectedBlockY] = shiftingBlocks[1];
            layout[selectedBlockX + 1, selectedBlockY] = shiftingBlocks[0];


            // Tell the board pieces to shift.
            Matrix block1World, block2World;
            block1World = shiftingBlocks[0].CurrentWorld;
            block2World = shiftingBlocks[1].CurrentWorld;

            shiftingBlocks[0].PrevLocation = block1World;
            shiftingBlocks[1].PrevLocation = block2World;

            shiftingBlocks[0].Destination = block2World;
            shiftingBlocks[1].Destination = block1World;

            shiftingBlocks[0].TransitionAmount = 0.0f;
            shiftingBlocks[1].TransitionAmount = 0.0f;
        }

        /// <summary>
        /// Swap the selected block with the block below it.
        /// </summary>
        public void SwapDown()
        {
            // Don't shift if the board is currently shifting.
            if (IsShifting) return;

            // Don't shift if the selected block is on the edge.
            if (selectedBlockY == 0) return;

            // Don't shift if there are no blocks to the left of the selected
            // block.
            if (!layout[selectedBlockX, selectedBlockY - 1].IsActive) return;

            // Tell the board to start shifting the selected blocks.
            shiftingBlocks.Add(layout[selectedBlockX, selectedBlockY]);
            shiftingBlocks.Add(layout[selectedBlockX, selectedBlockY - 1]);

            layout[selectedBlockX, selectedBlockY].IsSelected = false;
            layout[selectedBlockX, selectedBlockY - 1].IsSelected = true;

            // Swap positions in the layout array.
            layout[selectedBlockX, selectedBlockY] = shiftingBlocks[1];
            layout[selectedBlockX, selectedBlockY - 1] = shiftingBlocks[0];


            // Tell the board pieces to shift.
            Matrix block1World, block2World;
            block1World = shiftingBlocks[0].CurrentWorld;
            block2World = shiftingBlocks[1].CurrentWorld;

            shiftingBlocks[0].PrevLocation = block1World;
            shiftingBlocks[1].PrevLocation = block2World;

            shiftingBlocks[0].Destination = block2World;
            shiftingBlocks[1].Destination = block1World;

            shiftingBlocks[0].TransitionAmount = 0.0f;
            shiftingBlocks[1].TransitionAmount = 0.0f;
        }

        /// <summary>
        /// Swap the selected block with the one above it.
        /// </summary>
        public void SwapUp()
        {
            // Don't shift if the board is currently shifting.
            if (IsShifting) return;

            // Don't shift if the selected block is on the edge.
            if (selectedBlockY == Height - 1) return;

            // Don't shift if there are no blocks above the selected
            // block.
            if (!layout[selectedBlockX, selectedBlockY + 1].IsActive) return;

            // Tell the board to start shifting the selected blocks.
            shiftingBlocks.Add(layout[selectedBlockX, selectedBlockY]);
            shiftingBlocks.Add(layout[selectedBlockX, selectedBlockY + 1]);

            layout[selectedBlockX, selectedBlockY].IsSelected = false;
            layout[selectedBlockX, selectedBlockY + 1].IsSelected = true;

            // Swap positions in the layout array.
            layout[selectedBlockX, selectedBlockY] = shiftingBlocks[1];
            layout[selectedBlockX, selectedBlockY + 1] = shiftingBlocks[0];


            // Tell the board pieces to shift.
            Matrix block1World, block2World;
            block1World = shiftingBlocks[0].CurrentWorld;
            block2World = shiftingBlocks[1].CurrentWorld;

            shiftingBlocks[0].PrevLocation = block1World;
            shiftingBlocks[1].PrevLocation = block2World;

            shiftingBlocks[0].Destination = block2World;
            shiftingBlocks[1].Destination = block1World;

            shiftingBlocks[0].TransitionAmount = 0.0f;
            shiftingBlocks[1].TransitionAmount = 0.0f;
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
