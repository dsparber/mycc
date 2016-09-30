using constants;
using MyCryptos.resources;
using Xamarin.Forms;

namespace view.components
{
	public class CustomViewCell : SortableViewCell
	{
		protected readonly Label MasterLabel;
		protected readonly Label DetailLabel;
		protected readonly StackLayout LoadingView;
		protected readonly Image AccessoryImage;

		string text { get; set; }
		string detail { get; set; }
		string image { get; set; }
		bool isLoading { get; set; }

		public string Text
		{
			get { return text; }
			set { text = value; MasterLabel.Text = text; }
		}
		public string Detail
		{
			get { return detail; }
			set { detail = value; DetailLabel.Text = detail; }
		}

		public string Image
		{
			get { return image; }
			set { image = value; AccessoryImage.Source = ImageSource.FromFile(image); }
		}

		public bool IsLoading
		{
			get { return isLoading; }
			set { isLoading = value; DetailLabel.IsVisible = !value; LoadingView.IsVisible = value; }
		}

		public bool IsActionCell
		{
			get { return MasterLabel.TextColor.Equals(AppConstants.ThemeColor); }
			set { MasterLabel.TextColor = value ? AppConstants.ThemeColor : Color.Black; }
		}

		public CustomViewCell()
		{
			MasterLabel = new Label();
			DetailLabel = new Label { TextColor = Color.Gray, FontSize = MasterLabel.FontSize * 0.75 };
			LoadingView = new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 0, Padding = new Thickness(0), Margin = new Thickness(0) };
			LoadingView.Children.Add(new Label { Text = InternationalisationResources.RefreshingDots, TextColor = Color.Gray, FontSize = MasterLabel.FontSize * 0.75, VerticalOptions = LayoutOptions.Center });

			var stack = new StackLayout { Spacing = 0 };
			stack.Children.Add(MasterLabel);
			stack.Children.Add(DetailLabel);
			stack.Children.Add(LoadingView);

			AccessoryImage = new Image { HeightRequest = 20, HorizontalOptions = LayoutOptions.EndAndExpand };

			var mainView = new StackLayout { Orientation = StackOrientation.Horizontal, Margin = new Thickness(15, 0), VerticalOptions = LayoutOptions.CenterAndExpand };
			mainView.Children.Add(stack);
			mainView.Children.Add(AccessoryImage);

			View = mainView;

			IsLoading = false;
		}

		public override decimal Units { get { return 0; } }
		public override string Name { get { return Text; } }
		public override decimal Value { get { return 0; } }
	}
}