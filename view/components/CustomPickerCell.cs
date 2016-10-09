using constants;
using Xamarin.Forms;

namespace MyCryptos.view.components
{
    class CustomPickerCell : ViewCell
    {
        public readonly Picker Picker;
        protected readonly Label TitleLabel;

        string title;
        string placeholder;

        public string Title
        {
            get { return title; }
            set { title = value; TitleLabel.Text = title; }
        }

        public bool IsEditable
        {
            set
            {
                Picker.IsEnabled = value;
                Picker.Opacity = value ? 1 : 0.5;
            }
        }

        public CustomPickerCell()
        {
            Picker = new Picker { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
            TitleLabel = new Label { WidthRequest = 100, VerticalOptions = LayoutOptions.CenterAndExpand };

            var stack = new StackLayout { Orientation = StackOrientation.Horizontal, Margin = new Thickness(15, 0), VerticalOptions = LayoutOptions.CenterAndExpand };
            stack.Children.Add(TitleLabel);
            stack.Children.Add(Picker);


            if (Device.OS == TargetPlatform.Android)
            {
                TitleLabel.FontSize = AppConstants.AndroidFontSize;
            }

            View = stack;

            var gestureRecogniser = new TapGestureRecognizer();
            gestureRecogniser.Tapped += (sender, e) => Picker.Focus();
            View.GestureRecognizers.Add(gestureRecogniser);
        }
    }
}
