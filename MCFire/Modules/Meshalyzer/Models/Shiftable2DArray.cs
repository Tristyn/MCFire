using System;
using System.Collections;
using System.Collections.Generic;

namespace MCFire.Modules.Meshalyzer.Models
{
    [Obsolete]
    public class Shiftable2DArray<T> : IEnumerable<T>
    {
        readonly T[,] _array;
        readonly int _length;
        readonly int _width;
        int _offsetLength;
        int _offsetWidth;

        /// <summary>
        /// Creates a Shiftable2DArray with the specified length and width
        /// </summary>
        /// <param name="length">The length of the shiftable 2D array</param>
        /// <param name="width">The width of the shiftable 2D array</param>
        public Shiftable2DArray(int length, int width)
        {
            _array = new T[length, width];
            _length = length;
            _width = width;
        }

        public Shiftable2DArray(T[,] array)
            : this(array.GetLength(0), array.GetLength(1))
        {
            array.CopyTo(_array,0);
        }

        public T this[int i, int j]
        {
            // note: dont shift j, it gets shifted internally during the index.
            get { return _array[Mod(i + _offsetLength, _length), Mod(j + _offsetWidth, _width)]; }
        }

        /// <summary>
        /// Relatively shifts of the array by deltaLength and deltaWidth.
        /// </summary>
        /// <param name="deltaLength">The amount to shift in the first dimension.</param>
        /// <param name="deltaWidth">The amount to shift in the second dimension.</param>
        public void Shift(int deltaLength, int deltaWidth)
        {
            _offsetLength = Mod(_offsetLength + deltaLength, _length);
            _offsetWidth = Mod(_offsetWidth + deltaWidth, _width);
        }

        /// <summary>
        /// Sets the shift to absolute values.
        /// </summary>
        public void SetShift(int shiftLength, int shiftWidth)
        {
            _offsetLength = Mod(shiftLength, _length);
            _offsetWidth = Mod(shiftWidth, _width);
        }

        static int Mod(int x, int m)
        {
            var r = x % m;
            return r < 0 ? r + m : r;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)_array.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _array.GetEnumerator();
        }
    }
}
