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
        private Piece[,] layout;


        /// <summary>
        /// We keep a sequential copy of all of the pieces for fast iteration.
        /// </summary>
        private List<Piece> pieces = new List<Piece>();
        /// <summary>
        /// A collection of pieces on the board.
        /// </summary>
        public IEnumerable<Piece> Pieces
        {
            get { return pieces; }
        }

        /// <summary>
        /// 2D array of Matrices representing the position transforms
        /// to align each Piece into a grid.
        /// </summary>
        private Matrix[,] layoutTransforms;
        
        public Board2D(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
}
