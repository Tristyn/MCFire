﻿using MCFire.Modules.Infrastructure.Models;
using Substrate.Core;

namespace MCFire.Modules.Infrastructure.Extensions
{
    public static class ChunkExtensions
    {
        public static ChunkPosition Position(this IChunk chunk)
        {
            return new ChunkPosition(chunk.X, chunk.Z);
        }

        public static ChunkSize Size(this IChunk chunk)
        {
            return new ChunkSize(chunk);
        }
    }
}
