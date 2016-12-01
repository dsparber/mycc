using constants;
using Xamarin.Forms;

namespace MyCryptos.view.components
{
	class CustomPickerCell : ViewCell
	{
		public readonly Picker Picker;
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
				Picker.IsEnabled = value;
				Picker.Opacity = value ? 1 : 0.5;
			}
		}

		public CustomPickerCell()
		{
			Picker = new Picker { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
			TitleLabel = new Label { WidthRequest = AppConstants.LabelWidth, VerticalOptions = LayoutOptions.CenterAndExpand, TextColor = Color.FromHex("222") };

			var stack = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0), VerticalOptions = LayoutOptions.CenterAndExpand };
			stack.Children.Add(TitleLabel);
			stack.Children.Add(Picker);

			if (Device.OS == TargetPlatform.iOS)
			{
				var icon = new Image { Source = ImageSource.FromFile("down.png"), HeightRequest = 20, HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
				stack.Children.Add(icon);
			}


			if (Device.OS == TargetPlatform.Android)
			{
				TitleLabel.FontSize = AppConstants.AndroidFontSize;
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
			gestureRecogniser.Tapped += (sender, e) => Picker.Focus();
			View.GestureRecognizers.Add(gestureRecogniser);
		}
	}
}
