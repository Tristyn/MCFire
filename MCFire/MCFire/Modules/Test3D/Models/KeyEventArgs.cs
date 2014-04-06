using System;
using SharpDX;

namespace MCFire.Modules.Test3D.Models
{
    public class KeyEventArgs : EventArgs
    {
        public KeyState State { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 PrevPosition { get; set; }

        public KeyEventArgs(KeyState state,Vector2 position, Vector2 prevPosition)
        {
            State = state;
            Position = position;
            PrevPosition = prevPosition;
        }
    }
}
