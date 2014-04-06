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
            mouse.Initialize();
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
            Right.Update(state.Right, pos);
            Middle.Update(state.Middle, pos);
            State = State;
        }

        public void Dispose()
        {
            _mouse.Dispose();
            _mouse = null;
        }
    }

    public class Key
    {
        Vector2 _previousPosition;
        KeyState _previousState;

        public void Update(ButtonState state, Vector2 position)
        {
            Console.WriteLine(_previousPosition);
            switch (state)
            {
                case ButtonState.Pressed:
                    if (_previousState == KeyState.Chillin)
                    {
                        if (Click != null) Click(this, new KeyEventArgs(KeyState.Pressed, position, _previousPosition));
                        _previousState = KeyState.Pressed;
                        if (position != _previousPosition)
                        {
                            if (DragStart != null) DragStart(this, new KeyEventArgs(KeyState.Dragging, position, _previousPosition));
                            _previousState = KeyState.Dragging;
                        }
                    }
                    if (_previousState == KeyState.Pressed)
                    {
                        if (position != _previousPosition)
                        {
                            if (DragStart != null)
                                DragStart(this, new KeyEventArgs(KeyState.Dragging, position, _previousPosition));
                            _previousState = KeyState.Dragging;
                        }
                        else
                        {
                            _previousState = KeyState.Pressed;
                        }
                    }
                    if (_previousState == KeyState.Dragging && position != _previousPosition)
                    {
                        if (DragMove != null) DragMove(this, new KeyEventArgs(KeyState.Dragging, position, _previousPosition));
                        _previousState = KeyState.Dragging;
                    }
                    break;

                case ButtonState.Released:
                    if (_previousState == KeyState.Pressed)
                    {
                        if (ClickEnd != null) ClickEnd(this, new KeyEventArgs(KeyState.Released, position, _previousPosition));
                        _previousState = KeyState.Released;
                    }
                    if (_previousState == KeyState.EndDragging)
                    {
                        if (DragEnd != null) DragEnd(this, new KeyEventArgs(KeyState.EndDragging, position, _previousPosition));
                        // NOTE THE CHILLIN, not EndDragging!
                        _previousState = KeyState.Chillin;
                    }
                    if (_previousState == KeyState.Released)
                    {
                        _previousState = KeyState.Chillin;
                    }
                    if (_previousState == KeyState.Chillin)
                    {
                        _previousState = KeyState.Chillin;
                    }
                    break;
            }
            _previousPosition = position;
        }

        public event EventHandler<KeyEventArgs> Click;
        public event EventHandler<KeyEventArgs> DragStart;
        public event EventHandler<KeyEventArgs> DragMove;
        public event EventHandler<KeyEventArgs> DragEnd;
        public event EventHandler<KeyEventArgs> ClickEnd;
    }

    public enum KeyState
    {
        Pressed,
        Dragging,
        Released,
        EndDragging,
        Chillin
    }
}
