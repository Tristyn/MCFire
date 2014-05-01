using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using MCFire.Modules.Editor.Actions;
using MCFire.Modules.Explorer.Models;
using MCFire.Modules.Explorer.Views;
using MCFire.Modules.Infrastructure.Enums;
using Substrate;

namespace MCFire.Modules.Explorer.ViewModels
{
    public class WorldViewModel
    {
        MCFireWorld _model;

        public WorldViewModel()
        {
            Dimensions = new BindableCollection<DimensionViewModel>();
        }
        public void OpenEditorTo(DimensionViewModel source)
        {
            // hacky, but i love it.
            var enumerator = new[] {new OpenEditorTo(source.World, (int) source.Dimension)}.Cast<IResult>().GetEnumerator();
            Coroutine.BeginExecute(enumerator);
        }

        /// <summary>
        /// Deselects the current item when it looses focus.
        /// Prevents ugly ghost selections in the UI.
        /// </summary>
        public void DeselectItem(ListBox dataContext)
        {
            dataContext.SelectedIndex = -1;
        }

        public MCFireWorld Model
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
