using System;
using MCFire.Modules.Infrastructure.Models;

namespace MCFire.Modules.BoxSelector.Models
{
    struct BoxSelection
    {
        public BoxSelection(BlockPosition cornerOne, BlockPosition cornerTwo) : this()
        {
            CornerOne = cornerOne;
            CornerTwo = cornerTwo;
        }


        /// <summary>
        /// The original first corner used to specify the bounds of the box.
        /// </summary>
        public BlockPosition CornerOne { get; private set; }

        /// <summary>
        /// The original second corner used to specify the bounds of the box.
        /// </summary>
        public BlockPosition CornerTwo { get; private set; }

        /// <summary>
        /// The lesser corner of the selection. All fields are less than SelectionGreater
        /// </summary>
        public BlockPosition Lesser
        {
            get { return new BlockPosition(Left, Bottom, Backward); }
        }

        /// <summary>
        /// The greater corner of the selection. All fields are greater than SelectionLesser
        /// </summary>
        public BlockPosition Greater
        {
            get { return new BlockPosition(Right, Top, Forward); }
        }

        /// <summary>
        /// Length of the selection on the X axis (zero based like a list length)
        /// </summary>
        public int XLength { get { return Math.Abs(Right - Left) + 1; } } // + 1 because it is technically a collection length
        /// <summary>
        /// Height of the selection on the Y axis (zero based like a list length)
        /// </summary>
        public int YLength { get { return Math.Abs(Top - Bottom) + 1; } }
        /// <summary>
        /// Width of the selection on the Z axis (zero based like a list length)
        /// </summary>
        public int ZLength { get { return  Math.Abs(Forward - Backward) + 1; } }

        /// <summary>
        /// The X value of the right-most (1,0,0) face of the box.
        /// </summary>
        public int Right { get { return Math.Max(CornerOne.X, CornerTwo.X); } }
        /// <summary>
        /// The X value of the left-most (-1,0,0) face of the box.
        /// </summary>
        public int Left { get { return Math.Min(CornerOne.X, CornerTwo.X); } }
        /// <summary>
        /// The Y value of the top-most (0,1,0) face of the box.
        /// </summary>
        public int Top { get { return Math.Max(CornerOne.Y, CornerTwo.Y); } }
        /// <summary>
        /// The Y value of the bottom-most (0,-1,0) face of the box.
        /// </summary>
        public int Bottom { get { return Math.Min(CornerOne.Y, CornerTwo.Y); } }
        /// <summary>
        /// The Z value of the forward-most (0,0,1) face of the box.
        /// </summary>
        public int Forward { get { return Math.Max(CornerOne.Z, CornerTwo.Z); } }
        /// <summary>
        /// The Z value of the back-most (0,0,-1) face of the box.
        /// </summary>
        public int Backward { get { return Math.Min(CornerOne.Z, CornerTwo.Z); } }
    }
}
