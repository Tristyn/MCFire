using System.ComponentModel.Composition;
using MCFire.Graphics.Editor;
using SharpDX;
using SharpDX.Toolkit;

namespace MCFire.Graphics.Components
{
    [Export(typeof(IGameComponent)), PartCreationPolicy(CreationPolicy.NonShared)]
    class DuplicateComponent : ToolComponentBase
    {
        bool _lastSelected;

        [Import] WorldRenderer _world;

        protected override void LoadContent()
        {
            _world.LoadContent(Game);
            _world.Policy = ChunkCreationPolicy.Idle;
        }

        public override void Dispose()
        {
            _world.UnloadContent();
        }

        public override void Update(GameTime time)
        {
            if (Selected && !_lastSelected)
                CreateDuplicateVisual();

            _lastSelected = Selected;
        }

        void CreateDuplicateVisual()
        {
            var boxSelect = Game.GetComponent<BoxSelectorComponent>();
            if(boxSelect==null||boxSelect.SelectionState!= SelectionState.Set)
                return;

            var selection = boxSelect.Selection;
            _world.InclusiveZone = selection;
            _world.WorldMatrix = Matrix.Identity;
            _world.Policy= ChunkCreationPolicy.Run;
        }

        [Import(typeof(DuplicateTool))]
        protected override IEditorTool Tool { get; set; }
    }
}
