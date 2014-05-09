using System;
using System.Linq;
using MCFire.Modules.Infrastructure.Models;
using SharpDX;
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
        /// <param name="screenCoord">The coordinate in screen space (0 to 1). You can use the mouse coordinate to get the block that is under the mouse.</param>
        /// <param name="position">The output position if the screen coordinate intersected any blocks.</param>
        /// <returns>If the raytrace returned any blocks.</returns>
        public bool TryGetBlockAtScreenCoord(Vector2 screenCoord, out BlockPosition position)
        {
            bool exitedSolid = false;
            bool exitedAir = false;
            bool includeLiquids = false;
            bool firstEnumeration = true;

            foreach (var traceData in _game.Camera.RayTraceScreenPoint(screenCoord))
            {
                if (firstEnumeration)
                {
                    // check the first block that the trace is in.
                    // If its water, we dont include water in the search
                    var firstBlock = traceData.Blocks.FirstOrDefault();
                    includeLiquids = firstBlock == null || firstBlock.Info.State != BlockState.FLUID;
                    firstEnumeration = false;
                }

                if (traceData.Positions == null || traceData.Blocks == null) continue;
                for (int i = 0; i < traceData.Blocks.Count; i++)
                {
                    var block = traceData.Blocks[i];
                    // enumerate until it isn't a solid
                    if (!exitedSolid)
                    {
                        if (block.Info.State != BlockState.SOLID)
                            exitedSolid = true;
                        else continue;
                    }

                    // enumerate until it isn't air
                    if (!exitedAir)
                        if (block.ID != 0)
                            exitedAir = true;
                        else continue;

                    if (includeLiquids)
                    {
                        // enumerate to any block that isn't air
                        if (block.ID == 0) continue;
                        position = new BlockPosition(traceData.ChunkPosition, traceData.Positions[i], traceData.Size);
                        return true;
                    }

                    // enumerate to the first solid or nonsolid
                    if (block.Info.State == BlockState.FLUID || block.ID == 0) continue;

                    position = new BlockPosition(traceData.ChunkPosition, traceData.Positions[i], traceData.Size);
                    return true;
                }
            }
            position = new BlockPosition();
            return false;
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
