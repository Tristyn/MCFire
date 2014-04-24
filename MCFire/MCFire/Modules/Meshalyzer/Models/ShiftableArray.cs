using System.Collections;
using System.Collections.Generic;

namespace MCFire.Modules.Meshalyzer.Models
{
    /// <summary>
    /// An array that can have its contents shifted.
    /// </summary>
    public class ShiftableArray<T> : IEnumerable<T>
    {
        readonly T[] _array;
        readonly int _length;
        int _offset;

        public ShiftableArray(int length)
        {
            _array = new T[length];
            _length = length;
        }
        
        public T this[int index]
        {
            get { return _array[Mod(index + _offset, _length)]; }
            set { _array[Mod(index + _offset,_length)] = value; }
        }

        /// <summary>
        /// Relatively shifts the array by delta.
        /// </summary>
        /// <param name="delta">The amount of indexes to shift.</param>
        public void Shift(int delta)
        {
            _offset = Mod(_offset + delta, _length);
        }

        /// <summary>
        /// Sets the shift to an absolute value.
        /// </summary>
        /// <param name="shift"></param>
        public void SetShift(int shift)
        {
            _offset = Mod(shift, _length);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)_array.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _array.GetEnumerator();
        }

        static int Mod(int x, int m)
        {
            var r = x % m;
            return r < 0 ? r + m : r;
        }
    }
}
