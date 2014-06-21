using System.Collections.Generic;
using JetBrains.Annotations;
using MCFire.Common;
using MCFire.Graphics.Components;
using SharpDX.Toolkit;

namespace MCFire.Graphics.Editor
{
    public class EditorGameFactory
    {
        public IEditorGameFacade Create([NotNull] SharpDXElement context, [NotNull] IEnumerable<IGameComponent> components,
            [NotNull] World world, int dimension)
        {
            var game = new EditorGame(context, components, world, dimension);
            game.Run(context);
            return game;
        }
    }
}
