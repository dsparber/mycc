using MyCC.Forms.Constants;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.Cells
{
    public class CustomEntryCell : ViewCell
    {
        public readonly Entry Entry;
        protected readonly Label TitleLabel;

        private string _title;
        private string _placeholder;

        public string Title
        {
            get { return _title; }
            set { _title = value; TitleLabel.Text = _title; }
        }

        public string Placeholder
        {
            get { return _placeholder; }
            set { _placeholder = value; Entry.Placeholder = _placeholder; }
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
            TitleLabel = new Label { WidthRequest = AppConstants.LabelWidth, VerticalOptions = LayoutOptions.CenterAndExpand, TextColor = AppConstants.FontColor, LineBreakMode = LineBreakMode.NoWrap };

            var stack = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0), VerticalOptions = LayoutOptions.CenterAndExpand };
            stack.Children.Add(TitleLabel);
            stack.Children.Add(Entry);


            if (Device.OS == TargetPlatform.Android)
            {
                TitleLabel.FontSize = AppConstants.AndroidFontSize;
                Entry.FontSize = AppConstants.AndroidFontSize;
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
            gestureRecogniser.Tapped += (sender, e) => Entry.Focus();
            View.GestureRecognizers.Add(gestureRecogniser);
        }
    }
}

