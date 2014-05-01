using System.ComponentModel.Composition;
using JetBrains.Annotations;
using MCFire.Modules.Editor.Models;
using MCFire.Modules.Infrastructure.Models;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Buffer = SharpDX.Toolkit.Graphics.Buffer;
using Texture2D = SharpDX.Toolkit.Graphics.Texture2D;

namespace MCFire.Modules.BoxSelector.Models
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IGameComponent))]
    public class BoxSelectorComponent : GameComponentBase
    {
        [NotNull]
        Mesh<VertexPositionTexture> _boxMesh;
        [NotNull]
        BoxSelectEffect _effect;
        [NotNull]
        Texture2D _gridTexture;

        BoundingBox _selection = new BoundingBox(new Vector3(50,100,50), new Vector3(-1));
        Vector2 _texScale = new Vector2(1);

        public override void LoadContent()
        {
            var vertices = Buffer.Vertex.New(
                Game.GraphicsDevice,
                GeometricPrimitives.QuadVertexPositionTexture);
            _effect = new BoxSelectEffect(Game.LoadContent<Effect>("BoxSelect"))
            {
                Sampler = GraphicsDevice.SamplerStates.PointWrap
            };
            _gridTexture = Game.LoadContent<Texture2D>("Grid");
            _boxMesh = new Mesh<VertexPositionTexture>(vertices, _effect.Effect, true);
        }

        public override void Update(GameTime time)
        {
            // TODO:
            //_selection.Maximum = Tasks.GetBlockUnderMouse();
        }

        public override void Draw(GameTime time)
        {
            _effect.TransformMatrix = Camera.ViewMatrix * Camera.ProjectionMatrix;
            _effect.MainShift = new Vector2(_selection.Minimum.X, _selection.Minimum.Z);
            _effect.MainScale = new Vector2((_selection.Maximum.X - _selection.Minimum.X) / 16, (_selection.Maximum.Z - _selection.Minimum.Z)/16);
            _effect.Main = _gridTexture;
            GraphicsDevice.SetBlendState(GraphicsDevice.BlendStates.AlphaBlend);
            _boxMesh.Draw(GraphicsDevice);
        }

        public override int DrawPriority { get { return 500; } }

        public override void Dispose()
        {
            _boxMesh.Dispose();
        }
    }
}
