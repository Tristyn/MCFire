using System;
using MCFire.Modules.Infrastructure.Extensions;
using MCFire.Modules.Infrastructure.Models;
using SharpDX.Toolkit;
using Substrate;

namespace MCFire.Modules.Editor.Models
{
    /// <summary>
    /// High level common tasks when using an EditorGame
    /// </summary>
    public class Tasks
    {
        readonly EditorGame _game;

        public Tasks(EditorGame game)
        {
            _game = game;
        }

        /// <summary>
        /// Returns the block that the users mouse is hovering over.
        /// The rules are that if we are in a solid, we will jump to the first nonsolid.
        /// At that point, we will select the first non-air block
        /// An exception is that if we start in a liquid, we also ignore liquids (so you can select blocks while underwater)
        /// </summary>
        public Point3 GetBlockUnderMouse()
        {
            var tracer = _game.Camera.RayTraceScreenPoint(_game.Mouse.Position);
            // We are going to man-handle the enumerator for greater control over block selection.
            // The rules are that if we are in a solid, we will jump to the first nonsolid.
            // At that point, we will select the first non-air block
            // An exception is that if we start in a liquid, we also ignore liquids (so you can select blocks while underwater)
            var enumerator = tracer.GetEnumerator();

            var blocks = _game.World.NbtWorld.GetBlockManager(_game.Dimension);
            var state = blocks.GetBlock(enumerator.Current).Info.State;

            // enumerate until it isn't a solid
            if (state == BlockState.SOLID)
                while (enumerator.MoveNext())
                {
                    var block = blocks.GetBlock(enumerator.Current);
                    if (block == null || block.Info.State != BlockState.SOLID) break;
                }

            if (state == BlockState.NONSOLID)
                while (enumerator.MoveNext())
                {
                    // enumerate to the first solid or liquids
                    var block = blocks.GetBlock(enumerator.Current);
                    if (block == null || block.Info.State != BlockState.NONSOLID) break;
                }

            // enumerate to the first solid (ignore liquids)
            else while (enumerator.MoveNext())
                {
                    var block = blocks.GetBlock(enumerator.Current);
                    if (block == null || block.Info.State == BlockState.SOLID) break;
                }
            Console.WriteLine(enumerator.Current);
            return enumerator.Current;
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
