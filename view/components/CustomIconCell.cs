using constants;
using Xamarin.Forms;

namespace view.components
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
			textLabel = new Label();
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
			stack.Margin = new Thickness(15, 0);

			View = stack;
		}
	}
}


