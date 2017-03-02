using MyCC.Forms.View.Components.CellViews;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.Cells
{
    public class CustomSwitchCell : ViewCell
    {
        private readonly CustomSwitchView _view;

        public CustomSwitchCell()
        {
            _view = new CustomSwitchView();

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

        public string Title { set { _view.Title = value; } }

        public string Info { set { _view.Info = value; } }

        public bool On { set { _view.On = value; } get { return _view.On; } }

        public Switch Switch => _view.Switch;
    }
}
