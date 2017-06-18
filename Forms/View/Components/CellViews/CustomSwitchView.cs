using MyCC.Forms.Constants;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.CellViews
{
    public sealed class CustomSwitchView : ContentView
    {
        public readonly Switch Switch;
        public readonly Xamarin.Forms.View Panel;
        private readonly Label _titleLabel;
        private readonly Label _infoLabel;

        private string _title;
        private string _info;

        public string Title
        {
            get => _title;
            set { _title = value; _titleLabel.Text = _title; }
        }

        public string Info
        {
            get => _info;
            set { _info = value; _infoLabel.Text = _info; }
        }

        public bool IsEditable
        {
            set => Switch.IsEnabled = value;
        }

        public bool On
        {
            get => Switch.IsToggled;
            set => Switch.IsToggled = value;
        }

        public CustomSwitchView()
        {
            Switch = new Switch { HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };

            _titleLabel = new Label { HorizontalOptions = LayoutOptions.FillAndExpand, TextColor = AppConstants.FontColor, LineBreakMode = LineBreakMode.TailTruncation };
            _infoLabel = new Label { HorizontalOptions = LayoutOptions.FillAndExpand, TextColor = AppConstants.FontColorLight, LineBreakMode = LineBreakMode.TailTruncation, FontSize = _titleLabel.FontSize * AppConstants.FontFactorSmall };
            var labelStack = new StackLayout { Spacing = 0, VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };
            labelStack.Children.Add(_titleLabel);
            labelStack.Children.Add(_infoLabel);

            var stack = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0), VerticalOptions = LayoutOptions.CenterAndExpand, BackgroundColor = Color.White };
            stack.Children.Add(labelStack);
            stack.Children.Add(Switch);


            if (Device.RuntimePlatform.Equals(Device.Android))
            {
                _titleLabel.FontSize = AppConstants.AndroidFontSize;
            }

            stack.HorizontalOptions = LayoutOptions.FillAndExpand;
            stack.VerticalOptions = LayoutOptions.FillAndExpand;

            Panel = stack;

            Content = new ContentView { Content = new ContentView { Content = Panel, BackgroundColor = Color.White, Padding = new Thickness(0, 5) }, BackgroundColor = Color.FromHex("#ccc"), Padding = new Thickness(0, 0.5) };


            var gestureRecogniser = new TapGestureRecognizer();
            gestureRecogniser.Tapped += (sender, e) => Switch.Focus();
            Content.GestureRecognizers.Add(gestureRecogniser);
        }
    }
}
