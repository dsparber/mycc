using constants;
using Xamarin.Forms;

namespace MyCryptos.view.components
{
    class CustomSwitchCell : ViewCell
    {
        public readonly Switch Switch;
        protected readonly Label TitleLabel;

        string title;

        public string Title
        {
            get { return title; }
            set { title = value; TitleLabel.Text = title; }
        }

        public bool IsEditable
        {
            set
            {
                Switch.IsEnabled = value;
            }
        }

        public bool On
        {
            get { return Switch.IsToggled; }
            set { Switch.IsToggled = value; }
        }

        public CustomSwitchCell()
        {
            Switch = new Switch { HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };

            TitleLabel = new Label { VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };

            var stack = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0), VerticalOptions = LayoutOptions.CenterAndExpand };
            stack.Children.Add(TitleLabel);
            stack.Children.Add(Switch);


            if (Device.OS == TargetPlatform.Android)
            {
                TitleLabel.FontSize = AppConstants.AndroidFontSize;
            }

            stack.HorizontalOptions = LayoutOptions.FillAndExpand;
            stack.VerticalOptions = LayoutOptions.FillAndExpand;
            if (Device.OS == TargetPlatform.Android)
            {
                stack.BackgroundColor = Color.White;
                View = new ContentView { Content = stack, BackgroundColor = Color.FromHex("c7d7d4"), Padding = new Thickness(0, 0, 0, 0.5) };
            }
            else
            {
                View = stack;
            }

            var gestureRecogniser = new TapGestureRecognizer();
            gestureRecogniser.Tapped += (sender, e) => Switch.Focus();
            View.GestureRecognizers.Add(gestureRecogniser);
        }
    }
}
