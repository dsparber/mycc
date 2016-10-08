using constants;
using Xamarin.Forms;

namespace view.components
{
	public class CustomEntryCell : ViewCell
	{
		public readonly Entry Entry;
		protected readonly Label TitleLabel;

		string title;
		string placeholder;

		public string Title
		{
			get { return title; }
			set { title = value; TitleLabel.Text = title; }
		}

		public string Placeholder
		{
			get { return placeholder; }
			set { placeholder = value; Entry.Placeholder = placeholder; }
		}

		public string Text
		{
			get { return Entry != null ? Entry.Text : string.Empty; }
			set { Entry.Text = value; }
		}

		public bool IsEditable
		{
			set
			{
				Entry.IsEnabled = value;
				Entry.Opacity = value ? 1 : 0.5;
			}
		}

		public CustomEntryCell()
		{
			Entry = new Entry { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
			TitleLabel = new Label { WidthRequest = 100, VerticalOptions = LayoutOptions.CenterAndExpand };

			var stack = new StackLayout { Orientation = StackOrientation.Horizontal, Margin = new Thickness(15, 0), VerticalOptions = LayoutOptions.CenterAndExpand };
			stack.Children.Add(TitleLabel);
			stack.Children.Add(Entry);


            if (Device.OS == TargetPlatform.Android)
            {
                TitleLabel.FontSize = AppConstants.AndroidFontSize;
                Entry.FontSize = AppConstants.AndroidFontSize;
            }

            View = stack;

			var gestureRecogniser = new TapGestureRecognizer();
			gestureRecogniser.Tapped += (sender, e) => Entry.Focus();
			View.GestureRecognizers.Add(gestureRecogniser);
		}
	}
}

