using System;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using MCFire.Client.Gui.Modules.Editor.Actions;
using MCFire.Client.Primitives;
using MCFire.Common;
using Substrate;

namespace MCFire.Client.Gui.Modules.Explorer.ViewModels
{
    public class WorldViewModel
    {
        World _model;

        public WorldViewModel()
        {
            Dimensions = new BindableCollection<DimensionViewModel>();
        }

        public void OpenEditorTo(DimensionViewModel source)
        {
            // hacky, but i love it.
            var enumerator = new IResult[] {new OpenEditorTo(source.World, (int) source.Dimension)}.AsEnumerable();
            Coroutine.BeginExecute(enumerator.GetEnumerator());
        }

        /// <summary>
        /// Deselects the current item when it looses focus.
        /// Prevents ugly ghost selections in the UI.
        /// </summary>
        public void DeselectItem(ListBox dataContext)
        {
            dataContext.SelectedIndex = -1;
        }

        public World Model
        {
            get { return _model; }
            set
            {
                _model = value;
                foreach (var dimension in (DimensionType[])Enum.GetValues(typeof(DimensionType)))
                {
                    Dimensions.Add(new DimensionViewModel { Dimension = dimension, World = value });
                }
            }
        }

        public GameType GameType { get { return Model.GameType; } }
        public string Title { get { return Model.Title; } }
        public BindableCollection<DimensionViewModel> Dimensions { get; set; }
    }
}
