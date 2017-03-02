using System.Collections.Generic;
using MyCC.Forms.Constants;
using MyCC.Forms.Resources;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.CellViews
{
    public class CustomCellView : ContentView
    {
        private readonly Label _masterLabel;
        private readonly Label _detailLabel;
        private readonly StackLayout _loadingView;
        private readonly StackLayout _stack;
        private readonly StackLayout _actionItemsStack;
        private readonly Image _accessoryImage;

        private string _text;
        private string _detail;
        private string _image;
        private bool _isLoading;
        private List<CustomCellViewActionItem> _actions;

        public readonly ContentView Panel;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                Device.BeginInvokeOnMainThread(() =>
                {
                    _masterLabel.Text = _text;
                    _detailLabel.IsVisible = _detail != null;
                });
            }
        }
        public string Detail
        {
            get { return _detail; }
            set
            {
                _detail = value;
                Device.BeginInvokeOnMainThread(() =>
                {
                    _detailLabel.Text = _detail;
                    _detailLabel.IsVisible = !IsLoading;
                });
            }
        }

        public LineBreakMode DetailBreakMode
        {
            set
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    _detailLabel.LineBreakMode = value;
                });
            }
        }

        public string Image
        {
            set { _image = value; Device.BeginInvokeOnMainThread(() => _accessoryImage.Source = ImageSource.FromFile(_image)); }
        }

        public bool ShowIcon
        {
            set { Device.BeginInvokeOnMainThread(() => _accessoryImage.IsVisible = value); }
        }

        public List<CustomCellViewActionItem> ActionItems
        {
            set { _actions = value; SetActionItems(); }
            get { return _actions; }
        }

        private bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                Device.BeginInvokeOnMainThread(() =>
                {
                    _detailLabel.IsVisible = !value;
                    _loadingView.IsVisible = value;
                });
            }
        }

        public bool IsActionCell
        {
            set
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    _masterLabel.TextColor = value ? AppConstants.ThemeColor : AppConstants.FontColor;
                    if (value) _stack.Children.Remove(_detailLabel);
                });
            }
        }

        public bool IsDisaled
        {
            set
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    _masterLabel.TextColor = value ? AppConstants.FontColorLight : AppConstants.FontColor;
                });
            }
        }

        public bool IsDeleteActionCell
        {
            set
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    _masterLabel.TextColor = value ? Color.Red : AppConstants.FontColor;
                    if (value) _stack.Children.Remove(_detailLabel);
                    _masterLabel.HorizontalOptions = value ? LayoutOptions.CenterAndExpand : LayoutOptions.StartAndExpand;
                });
            }
        }

        public bool IsCentered
        {
            set { Device.BeginInvokeOnMainThread(() => _masterLabel.HorizontalOptions = value ? LayoutOptions.CenterAndExpand : LayoutOptions.StartAndExpand); }
        }

        public CustomCellView(bool isStandalone = false)
        {
            _masterLabel = new Label { TextColor = Color.FromHex("222"), LineBreakMode = LineBreakMode.TailTruncation, VerticalOptions = LayoutOptions.Center };

            if (Device.OS == TargetPlatform.Android)
            {
                _masterLabel.FontSize = AppConstants.AndroidFontSize;
            }

            _detailLabel = new Label { TextColor = Color.Gray, FontSize = _masterLabel.FontSize * 0.75, LineBreakMode = LineBreakMode.MiddleTruncation, VerticalOptions = LayoutOptions.Center };

            _loadingView = new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 0, Padding = new Thickness(0), Margin = new Thickness(0) };
            _loadingView.Children.Add(new Label { Text = I18N.RefreshingDots, TextColor = Color.Gray, FontSize = _masterLabel.FontSize * 0.75, VerticalOptions = LayoutOptions.Center });

            _stack = new StackLayout { Spacing = 0, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
            _stack.Children.Add(_masterLabel);
            _stack.Children.Add(_detailLabel);
            _stack.Children.Add(_loadingView);

            _accessoryImage = new Image { HeightRequest = 20, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.FillAndExpand };
            _actionItemsStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.FillAndExpand };

            var mainView = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0), VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.Fill };
            mainView.Children.Add(_stack);
            mainView.Children.Add(_actionItemsStack);
            mainView.Children.Add(_accessoryImage);

            var content = new ContentView { Content = mainView, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.White };

            Panel = content;
            Content = new ContentView { Content = new ContentView { Content = Panel, BackgroundColor = Color.White, Padding = new Thickness(0, isStandalone ? 10 : 5) }, BackgroundColor = Color.FromHex("#ccc"), Padding = new Thickness(0, isStandalone ? 0.5 : 0, 0, 0.5) };

            IsLoading = false;
        }

        private void SetActionItems()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _actionItemsStack.Children.Clear();
                foreach (var a in _actions)
                {
                    var img = new Image
                    {
                        HeightRequest = 20,
                        Source = ImageSource.FromFile(a.Icon),
                        VerticalOptions = LayoutOptions.Center
                    };
                    var content = new ContentView
                    {
                        Content = img,
                        Padding = new Thickness(10, 0),
                        VerticalOptions = LayoutOptions.FillAndExpand
                    };
                    var gestureRecognizer = new TapGestureRecognizer();
                    gestureRecognizer.Tapped += a.Action;
                    gestureRecognizer.CommandParameter = a.Data;
                    content.GestureRecognizers.Add(gestureRecognizer);
                    _actionItemsStack.Children.Add(content);
                }
            });
        }
    }
}