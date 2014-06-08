using System;

namespace MCFire.Graphics.Modules.Editor.Models
{
    public class Keyboard : IDisposable
    {
        KeyboardManager _keyboard;
        bool _disposed;
        private KeyboardState _state;

        public Keyboard(Game game)
        {
            _keyboard=new KeyboardManager(game);
            _keyboard.Initialize();
        }

        public void Update()
        {
            if (_disposed)
                throw new ObjectDisposedException("Keyboard");

            State = _keyboard.GetState();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _keyboard.Dispose();
            _keyboard = null;
        }

        public KeyboardState State
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("Keyboard");

                return _state;
            }
            private set { _state = value; }
        }
    }
}
