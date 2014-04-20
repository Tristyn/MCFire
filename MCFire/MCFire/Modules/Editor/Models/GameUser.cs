using System;
using System.Linq;
using SharpDX.Toolkit;
using Substrate;
using ButtonState = SharpDX.Toolkit.Input.ButtonState;

namespace MCFire.Modules.Editor.Models
{
    /// <summary>
    /// User interaction logic for EditorGame.
    /// </summary>
    public class GameUser
    {
        readonly EditorGame _game;
        readonly Camera _camera;
        readonly Mouse _mouse;
        readonly SharpDXElement _sharpDx;

        public GameUser(EditorGame game)
        {
            _game = game;
            _camera = game.Camera;
            _mouse = game.Mouse;
            _sharpDx = game.SharpDxElement;
        }

        public void Update(GameTime gameTime)
        {
            if (_mouse.Left.State == ButtonState.Released)
                return;

            var tracer = _camera.RayTraceScreenPoint(_mouse.Position);
            // We are going to man-handle the enumerator for greater control over block selection.
            // The rules are that if we are in a solid, we will jump to the first nonsolid.
            // At that point, we will select the first non-air block
            // An exception is that if we start in a liquid, we also ignore liquids (so you can select blocks while underwater)
            var enumerator = tracer.GetEnumerator();
            if (!enumerator.MoveNext()) return;
            var state = _game.GetBlock(enumerator.Current).Info.State;

            // enumerate until it isn't a solid
            if (state == BlockState.SOLID)
                while (enumerator.MoveNext())
                {
                    if (_game.GetBlock(enumerator.Current).Info.State != BlockState.SOLID) break;
                }

            if (state == BlockState.NONSOLID)
                while (enumerator.MoveNext())
                {
                    // enumerate to the first solid or liquids
                    if (_game.GetBlock(enumerator.Current).Info.State != BlockState.NONSOLID) break;
                }
            // enumerate to the first solid (ignore liquids)
            else while (enumerator.MoveNext())
                {
                    if (_game.GetBlock(enumerator.Current).Info.State == BlockState.SOLID) break;
                }
            // TODO: do something with it :/
            //Console.WriteLine(enumerator.Current);
        }
    }
}
