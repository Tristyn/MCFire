using System;
using System.Collections.Generic;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Modules.Test3D.Models
{
    public class ModelMesh
    {
        public List<ModelMeshPart> MeshParts { get; private set; }

        public ModelMesh()
        {
            MeshParts=new List<ModelMeshPart>();
        }

        /// <summary>
        /// Draws all of the ModelMeshPart objects in this mesh, using their current Effect settings.
        /// </summary>
        /// <param name="context">The graphics context.</param>
        /// <exception cref="System.InvalidOperationException">Model has no effect</exception>
        public void Draw(GraphicsDevice context)
        {
            var count = MeshParts.Count;
            for (var i = 0; i < count; i++)
            {
                MeshParts[i].Draw(context);
            }
        }
    }
}
