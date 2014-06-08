using System;

namespace MCFire.Graphics.Modules.Editor.Models
{
    public class KeyEventArgs : EventArgs
    {
        public Vector2 Position { get; set; }
        public Vector2 PrevPosition { get; set; }

        public KeyEventArgs(Vector2 position, Vector2 prevPosition)
        {
            Position = position;
            PrevPosition = prevPosition;
        }
    }
}
