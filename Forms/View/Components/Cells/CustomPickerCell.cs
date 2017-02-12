using MyCC.Forms.Constants;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.Cells
{
    internal class CustomPickerCell : ViewCell
    {
        private readonly Picker _picker;
        private readonly Label _titleLabel;

        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; _titleLabel.Text = _title; }
        }

        public bool IsEditable
        {
            set
            {
                _picker.IsEnabled = value;
                _picker.Opacity = value ? 1 : 0.5;
            }
        }

        public CustomPickerCell()
        {
            _picker = new Picker { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
            _titleLabel = new Label { WidthRequest = AppConstants.LabelWidth, VerticalOptions = LayoutOptions.CenterAndExpand, TextColor = Color.FromHex("222") };

            var stack = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0), VerticalOptions = LayoutOptions.CenterAndExpand };
            stack.Children.Add(_titleLabel);
            stack.Children.Add(_picker);

            if (Device.OS == TargetPlatform.iOS)
            {
                var icon = new Image { Source = ImageSource.FromFile("down.png"), HeightRequest = 20, HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
                stack.Children.Add(icon);
            }


            if (Device.OS == TargetPlatform.Android)
            {
                _titleLabel.FontSize = AppConstants.AndroidFontSize;
            }

            stack.HorizontalOptions = LayoutOptions.FillAndExpand;
            stack.VerticalOptions = LayoutOptions.FillAndExpand;
            if (Device.OS == TargetPlatform.Android)
            {
                stack.BackgroundColor = Color.White;
                View = new ContentView { Content = stack, BackgroundColor = Color.FromHex("c7d7d4"), Padding = new Thickness(0, 0, 0, 0.5), Margin = new Thickness(0, 0, 0, -0.5) };
            }
            else
            {
                View = stack;
            }

            var gestureRecogniser = new TapGestureRecognizer();
            gestureRecogniser.Tapped += (sender, e) => _picker.Focus();
            View.GestureRecognizers.Add(gestureRecogniser);
        }
    }
}
