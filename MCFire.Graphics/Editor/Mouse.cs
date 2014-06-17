using System;
using SharpDX;
using SharpDX.Toolkit.Input;

namespace MCFire.Graphics.Editor
{
    public class Mouse : IDisposable
    {
        MouseManager _mouse;

        public Mouse(MouseManager mouse)
        {
            _mouse = mouse;
            mouse.Initialize();
            MouseManager = mouse;
            Left = new Key();
            Right = new Key();
            Middle = new Key();
        }

        public void Update()
        {
            var state = _mouse.GetState();
            var pos = new Vector2(state.X, state.Y);
            Position = pos;
            
            Left.Update(state.LeftButton, pos);
            Right.Update(state.RightButton, pos);
            Middle.Update(state.MiddleButton, pos);
        }

        /// <summary>
        /// Moves the mouse to a new position. This will not fire any events or changed their arguments.
        /// </summary>
        /// <param name="position">The new mouse position in desktop space.</param>
        public void SetPosition(Vector2 position)
        {
            _mouse.SetPosition(position);

            Left.SetPosition(position);
            Right.SetPosition(position);
            Middle.SetPosition(position);
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
        public MouseManager MouseManager { get; private set; }
    }

    public class Key
    {
        bool _lastDown;
        Vector2 _previousPosition;
        bool _dragging;

        public void Update(ButtonState state, Vector2 position)
        {
            // we cant use the ButtonState alone, as the SharpDX Mouse gets 2 events per mouse click.
            State = state;
            // pressed this frame
            if (_lastDown==false && state.Down)
            {
                if (Click != null) Click(this, new KeyEventArgs(position, _previousPosition));
                if (position != _previousPosition)
                {
                    _dragging = true;
                    if (DragStart != null) DragStart(this, new KeyEventArgs(position, _previousPosition));
                }
            }
            // still held this frame
            else if (_lastDown&&state.Down)
            {
                if (position == _previousPosition)
                {
                    if (ClickHeld != null) ClickHeld(this, new KeyEventArgs(position, _previousPosition));
                }
                else if (_dragging)
                {
                    if (DragMove != null) DragMove(this, new KeyEventArgs(position, _previousPosition));
                }
                else
                {
                    _dragging = true;
                    if (DragStart != null) DragStart(this, new KeyEventArgs(position, _previousPosition));
                }
            }
            // released this frame
            else if (_lastDown && state.Down==false)
            {
                if (_dragging)
                {
                    _dragging = false;
                    if (DragEnd != null) DragEnd(this, new KeyEventArgs(position, _previousPosition));
                }
                else
                {
                    if (ClickEnd != null) ClickEnd(this, new KeyEventArgs(position, _previousPosition));
                }
            }
            // still chilling this frame
            //else if (_lastDown==false&&state.Down==false)

            _lastDown = state.Down;
            _previousPosition = position;
        }

        public void SetPosition(Vector2 position)
        {
            _previousPosition = position;
        }

        public ButtonState State { get; private set; }

        public event EventHandler<KeyEventArgs> Click;
        public event EventHandler<KeyEventArgs> ClickHeld;
        public event EventHandler<KeyEventArgs> ClickEnd;
        public event EventHandler<KeyEventArgs> DragStart;
        public event EventHandler<KeyEventArgs> DragMove;
        public event EventHandler<KeyEventArgs> DragEnd;
    }
}
