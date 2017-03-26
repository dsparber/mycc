using MyCC.Forms.Constants;
using MyCC.Forms.View.Components.BaseComponents;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.Cells
{
    public class CustomEntryCell : ViewCell
    {
        private readonly Entry _entry;
        private readonly NumericEntry _numericEntry;
        private bool _isNumeric;

        public Entry Entry => _isNumeric ? _numericEntry : _entry;

        public bool IsNumeric
        {
            get
            {
                return _isNumeric;
            }
            set
            {
                _isNumeric = value;
                _entry.IsVisible = !value;
                _numericEntry.IsVisible = value;
            }
        }

        private readonly Label _titleLabel;

        private string _title;
        private string _placeholder;

        public string Title
        {
            set { _title = value; _titleLabel.Text = _title; }
        }

        public string Placeholder
        {
            set
            {
                _placeholder = value;
                _numericEntry.Placeholder = _placeholder;
                _entry.Placeholder = _placeholder;
            }
        }

        public string Text
        {
            get { return Entry != null ? Entry.Text : string.Empty; }
            set { Entry.Text = value; }
        }

        public bool IsEditable
        {
            get { return Entry.IsEnabled; }
            set
            {
                _entry.IsEnabled = value;
                _entry.Opacity = value ? 1 : 0.5;
                _numericEntry.IsEnabled = value;
                _numericEntry.Opacity = value ? 1 : 0.5;
            }
        }

        public bool IsPin
        {
            get { return _numericEntry.IsPin; }
            set { _numericEntry.IsPin = value; }
        }

        public CustomEntryCell()
        {
            _numericEntry = new NumericEntry { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, IsVisible = false };
            _entry = new Entry { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };

            _titleLabel = new Label { WidthRequest = AppConstants.LabelWidth, MinimumWidthRequest = AppConstants.LabelWidth, VerticalOptions = LayoutOptions.CenterAndExpand, TextColor = AppConstants.FontColor, LineBreakMode = LineBreakMode.NoWrap };

            var stack = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0), VerticalOptions = LayoutOptions.CenterAndExpand };
            stack.Children.Add(_titleLabel);
            stack.Children.Add(_numericEntry);
            stack.Children.Add(_entry);


            if (Device.OS == TargetPlatform.Android)
            {
                stack.Spacing = 3;
                _titleLabel.FontSize = AppConstants.AndroidFontSize;
                _numericEntry.FontSize = AppConstants.AndroidFontSize;
                _entry.FontSize = AppConstants.AndroidFontSize;
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

            View.MinimumHeightRequest = 44;
            Height = 44;

            var gestureRecogniser = new TapGestureRecognizer();
            gestureRecogniser.Tapped += (sender, e) => Entry.Focus();
            View.GestureRecognizers.Add(gestureRecogniser);
        }
    }
}

