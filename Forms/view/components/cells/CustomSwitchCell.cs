using constants;
using Xamarin.Forms;

namespace MyCryptos.view.components
{
	public sealed class CustomSwitchCell : ViewCell
	{
		public readonly Switch Switch;
		private readonly Label titleLabel;
		private readonly Label infoLabel;

		private string title;
		private string info;

		public string Title
		{
			get { return title; }
			set { title = value; titleLabel.Text = title; }
		}

		public string Info
		{
			get { return info; }
			set { info = value; infoLabel.Text = info; }
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

			titleLabel = new Label { HorizontalOptions = LayoutOptions.FillAndExpand, TextColor = AppConstants.FontColor };
			infoLabel = new Label { HorizontalOptions = LayoutOptions.FillAndExpand, TextColor = AppConstants.FontColorLight, FontSize = titleLabel.FontSize * AppConstants.FontFactorSmall };
			var labelStack = new StackLayout { Spacing = 0, VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };
			labelStack.Children.Add(titleLabel);
			labelStack.Children.Add(infoLabel);

			var stack = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0), VerticalOptions = LayoutOptions.CenterAndExpand };
			stack.Children.Add(labelStack);
			stack.Children.Add(Switch);


			if (Device.OS == TargetPlatform.Android)
			{
				titleLabel.FontSize = AppConstants.AndroidFontSize;
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
