using System.Collections.Generic;
using MyCC.Forms.view.components.CellViews;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.Cells
{
    public class CustomViewCell : ViewCell
    {
        private readonly CustomCellView _view;

        public CustomViewCell()
        {
            _view = new CustomCellView();

            if (Device.OS == TargetPlatform.Android)
            {
                _view.Panel.BackgroundColor = Color.White;
                View = new ContentView { Content = _view.Panel, BackgroundColor = Color.FromHex("c7d7d4"), Padding = new Thickness(0, 0, 0, 0.5) };
            }
            else
            {
                View = _view.Panel;
            }

        }

        public string Text
        {
            set { _view.Text = value; }
            get { return _view.Text; }
        }

        public string Detail
        {
            set { _view.Detail = value; }
            get { return _view.Detail; }
        }

        public string Image
        {
            set { _view.Image = value; }
        }

        public List<CustomCellViewActionItem> ActionItems
        {
            get { return _view.ActionItems; }
            set { _view.ActionItems = value; }
        }

        public bool IsActionCell
        {
            set { _view.IsActionCell = value; }
        }

        public bool IsDeleteActionCell
        {
            set { _view.IsDeleteActionCell = value; }
        }

        public bool ShowIcon
        {
            set { _view.ShowIcon = value; }
        }

        public bool IsCentered
        {
            set { _view.IsCentered = value; }
        }

        public bool IsDisabled
        {
            set { _view.IsDisaled = value; }
        }

        public LineBreakMode DetailBreakMode
        {
            set { _view.DetailBreakMode = value; }
        }
    }
}
