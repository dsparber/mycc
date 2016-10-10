using constants;
using Xamarin.Forms;

namespace MyCryptos.view.components
{
    public class CustomIconCell : ViewCell
    {
        readonly Label textLabel;
        readonly Image iconImage;

        bool showIcon;

        public bool ShowIcon
        {
            get { return showIcon; }
            set { showIcon = value; iconImage.IsVisible = showIcon; }
        }

        public string Icon
        {
            set
            {
                var img = ImageSource.FromFile(value);
                iconImage.Source = img;
            }
        }
        public string Text
        {
            set { textLabel.Text = value; }
        }

        public CustomIconCell()
        {
            textLabel = new Label { TextColor = Color.FromHex("222") };
            iconImage = new Image { Aspect = Aspect.AspectFit };
            iconImage.IsVisible = ShowIcon;
            iconImage.HorizontalOptions = LayoutOptions.EndAndExpand;
            iconImage.HeightRequest = 20;


            if (Device.OS == TargetPlatform.Android)
            {
                textLabel.FontSize = AppConstants.AndroidFontSize;
            }

            var stack = new StackLayout { Orientation = StackOrientation.Horizontal };
            stack.Children.Add(textLabel);
            stack.Children.Add(iconImage);
            stack.VerticalOptions = LayoutOptions.CenterAndExpand;
            stack.Padding = new Thickness(15, 0);

            if (Device.OS == TargetPlatform.Android)
            {
                var view = new StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.White };
                view.Children.Add(stack);
                View = new ContentView { Content = view, BackgroundColor = Color.FromHex("c7d7d4"), Padding = new Thickness(0, 0, 0, 0.5)};
            }
            else
            {
                View = stack;
            }

        }
    }
}


