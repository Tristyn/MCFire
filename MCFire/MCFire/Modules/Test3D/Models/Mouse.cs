using System;
using SharpDX;
using SharpDX.Toolkit.Input;

namespace MCFire.Modules.Test3D.Models
{
    public class Mouse : IDisposable
    {
        MouseManager _mouse;

        public Mouse(MouseManager mouse)
        {
            _mouse = mouse;
            State = _mouse.GetState();
            Left = new Key();
            Right = new Key();
            Middle = new Key();
        }

        public Key Left { get; private set; }
        public Key Right { get; private set; }
        public Key Middle { get; private set; }
        public MouseState State { get; private set; }

        public void Update()
        {
            var state = _mouse.GetState();
            var pos = new Vector2(state.X, state.Y);

            Left.Update(state.Left, pos);
            Right.Update(state.Left, pos);
            Middle.Update(state.Left, pos);
            State = State;
        }

        public class Key
        {
            Vector2 _previousPosition;
            KeyState _previousState;

            public void Update(ButtonState state, Vector2 position)
            {
                switch (state)
                {
                    case ButtonState.Pressed:
                        if (_previousState == KeyState.Chillin)
                        {
                            _previousState = KeyState.Pressed;
                            if (Click != null) Click(this, EventArgs.Empty);
                            if (position != _previousPosition)
                            {
                                if (DragStart != null) DragStart(this, EventArgs.Empty);
                                _previousState = KeyState.Dragging;
                            }
                            _previousPosition = position;
                        }
                        if (_previousState == KeyState.Pressed && position != _previousPosition)
                        {
                            if (DragStart != null) DragStart(this, EventArgs.Empty);
                            _previousState = KeyState.Dragging;
                            _previousPosition = position;
                        }
                        if (_previousState == KeyState.Dragging && position != _previousPosition)
                        {
                            _previousState = KeyState.Dragging;
                            _previousPosition = position;
                            if (DragMove != null) DragMove(this, EventArgs.Empty);
                        }
                        break;

                    case ButtonState.Released:
                        if (_previousState == KeyState.Pressed)
                        {
                            if (ClickEnd != null) ClickEnd(this, EventArgs.Empty);
                            _previousState = KeyState.Released;
                            _previousPosition = position;
                        }
                        if (_previousState == KeyState.EndDragging)
                        {
                            if (DragEnd != null) DragEnd(this, EventArgs.Empty);
                            // NOTE THE CHILLIN, not EndDragging!
                            _previousState = KeyState.Chillin;
                            _previousPosition = position;
                        }
                        break;
                }
            }

            public event EventHandler Click;
            public event EventHandler DragStart;
            public event EventHandler DragMove;
            public event EventHandler DragEnd;
            public event EventHandler ClickEnd;
        }

        enum KeyState
        {
            Pressed,
            Dragging,
            Released,
            EndDragging,
            Chillin
        }

        public void Dispose()
        {
            _mouse = null;
        }
    }
}
