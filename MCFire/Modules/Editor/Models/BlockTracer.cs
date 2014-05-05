using System;
using JetBrains.Annotations;
using MCFire.Modules.Explorer.Models;

namespace MCFire.Modules.Editor.Models
{
    public class BlockTracer
    {
        [NotNull]
        readonly VoxelTracer _tracer;
        [NotNull]
        readonly MCFireWorld _world;

        public BlockTracer([NotNull] VoxelTracer tracer, [NotNull] MCFireWorld world)
        {
            throw new NotImplementedException();
            if (tracer == null) throw new ArgumentNullException("tracer");
            if (world == null) throw new ArgumentNullException("world");
            _tracer = tracer;
            _world = world;
        }
    }
}
