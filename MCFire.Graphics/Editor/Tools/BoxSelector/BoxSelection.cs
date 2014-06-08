using System;

namespace MCFire.Graphics.Modules.BoxSelector.Models
{
    public class BoxSelection
    {
        public BoxSelection(BlockPosition cornerOne, BlockPosition cornerTwo)
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
            get { return new BlockPosition(Left, Bottom, Forward); }
        }

        /// <summary>
        /// The greater corner of the selection. All fields are greater than SelectionLesser
        /// </summary>
        public BlockPosition Greater
        {
            get { return new BlockPosition(Right, Top, Backward); }
        }

        /// <summary>
        /// The X value of the right-most (1,0,0) face of the box.
        /// </summary>
        public int Right { get { return Math.Max((int) CornerOne.X, (int) CornerTwo.X); } }
        /// <summary>
        /// The X value of the left-most (-1,0,0) face of the box.
        /// </summary>
        public int Left { get { return Math.Min((int) CornerOne.X, (int) CornerTwo.X); } }
        /// <summary>
        /// The Y value of the top-most (0,1,0) face of the box.
        /// </summary>
        public int Top { get { return Math.Max((int) CornerOne.Y, (int) CornerTwo.Y); } }
        /// <summary>
        /// The Y value of the bottom-most (0,-1,0) face of the box.
        /// </summary>
        public int Bottom { get { return Math.Min((int) CornerOne.Y, (int) CornerTwo.Y); } }
        /// <summary>
        /// The Z value of the forward-most (0,0,-1) face of the box.
        /// </summary>
        public int Forward { get { return Math.Min((int) CornerOne.Z, (int) CornerTwo.Z); } }
        /// <summary>
        /// The Z value of the back-most (0,0,1) face of the box.
        /// </summary>
        public int Backward { get { return Math.Max((int) CornerOne.Z, (int) CornerTwo.Z); } }

        public Cuboid GetCuboid()
        {
            // lengths of the cuboid are floored at 1
            var pos1 = CornerOne;
            var pos2 = CornerTwo;

            var minX = Math.Min((int) pos1.X, (int) pos2.X);
            var minY = Math.Min((int) pos1.Y, (int) pos2.Y);
            var minZ = Math.Min((int) pos1.Z, (int) pos2.Z);
            var maxX = Math.Max((int) pos1.X, (int) pos2.X);
            var maxY = Math.Max((int) pos1.Y, (int) pos2.Y);
            var maxZ = Math.Max((int) pos1.Z, (int) pos2.Z);

            return new Cuboid(
                minX, minY, minZ,
                maxX == minX ? 1 : maxX - minX+1,
                maxY == minY ? 1 : maxY - minY+1,
                maxZ == minZ ? 1 : maxZ - minZ+1);
        }

        /// <summary>
        /// Returns if the position is inside of this selection
        /// </summary>
        public bool BlockWithin(BlockPosition position)
        {
            return position.X >= Left && position.X <= Right
                && position.Y >= Bottom && position.Y <= Top
                && position.Z >= Forward && position.Z <= Backward;
        }

        public override string ToString()
        {
            return String.Format((string) "Lesser: {0}, Greater: {1}", (object) Lesser, (object) Greater);
        }
    }
}
