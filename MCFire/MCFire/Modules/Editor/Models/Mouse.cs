using System;
using SharpDX;
using SharpDX.Toolkit.Input;

namespace MCFire.Modules.Editor.Models
{
    public class Mouse : IDisposable
    {
        MouseManager _mouse;

        public Mouse(MouseManager mouse)
        {
            _mouse = mouse;
            mouse.Initialize();

            Left = new Key();
            Right = new Key();
            Middle = new Key();
        }

        public void Update()
        {
            var state = _mouse.GetState();
            var pos = new Vector2(state.X, state.Y);

            Position = pos;
            Left.Update(state.Left, pos);
            Right.Update(state.Right, pos);
            Middle.Update(state.Middle, pos);
        }

        /// <summary>
        /// Moves the mouse to a new position. This will not fire any events or changed their arguments.
        /// </summary>
        /// <param name="position">The new mouse position in desktop space.</param>
        public void MoveSilently(Vector2 position)
        {
            _mouse.SetPosition(position);

            Left.IgnoreNextMoveEvent();
            Right.IgnoreNextMoveEvent();
            Middle.IgnoreNextMoveEvent();
        }

        public void Dispose()
        {
            _mouse.Dispose();
            _mouse = null;
        }

        public Key Left { get; private set; }
        public Key Right { get; private set; }
        public Key Middle { get; private set; }
        public Vector2 Position { get; private set; }
    }

    public class Key
    {
        Vector2 _previousPosition;
        KeyState _previousState;
        bool _ignoreNextMove;

        public void Update(ButtonState state, Vector2 position)
        {
            State = state;

            var previousPosition = GetPreviousPosition(position);
            switch (state)
            {
                case ButtonState.Pressed:
                    if (_previousState == KeyState.Chillin)
                    {
                        if (Click != null) Click(this, new KeyEventArgs(KeyState.Pressed, position, previousPosition));
                        _previousState = KeyState.Pressed;
                        if (position != previousPosition)
                        {
                            if (DragStart != null) DragStart(this, new KeyEventArgs(KeyState.Dragging, position, previousPosition));
                            _previousState = KeyState.Dragging;
                        }
                    }
                    if (_previousState == KeyState.Pressed)
                    {
                        if (position != previousPosition)
                        {
                            if (DragStart != null)
                                DragStart(this, new KeyEventArgs(KeyState.Dragging, position, previousPosition));
                            _previousState = KeyState.Dragging;
                        }
                        else
                        {
                            _previousState = KeyState.Pressed;
                        }
                    }
                    if (_previousState == KeyState.Dragging && position != previousPosition)
                    {
                        if (DragMove != null) DragMove(this, new KeyEventArgs(KeyState.Dragging, position, previousPosition));
                        _previousState = KeyState.Dragging;
                    }
                    break;

                case ButtonState.Released:
                    if (_previousState == KeyState.Pressed)
                    {
                        if (ClickEnd != null) ClickEnd(this, new KeyEventArgs(KeyState.Released, position, previousPosition));
                        _previousState = KeyState.Released;
                    }
                    if (_previousState == KeyState.Dragging)
                    {
                        if (DragEnd != null) DragEnd(this, new KeyEventArgs(KeyState.EndDragging, position, previousPosition));
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

        public void IgnoreNextMoveEvent()
        {
            _ignoreNextMove = true;
        }

        /// <summary>
        /// Returns currentPosition if _ignoreNextMove is set to true. else returns _previousPosition
        /// </summary>
        Vector2 GetPreviousPosition(Vector2 currentPosition)
        {
            if (!_ignoreNextMove)
                return _previousPosition;
            
            _ignoreNextMove = false;
            return currentPosition;
        }

        public ButtonState State { get; private set; }

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
