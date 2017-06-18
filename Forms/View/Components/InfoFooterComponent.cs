using MyCC.Forms.Constants;
using MyCC.Forms.Resources;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components
{
    public class InfoFooterComponent : ContentView
    {
        public string Text { set => _label.Text = value; }

        private string Warning
        {
            set
            {
                _warningView.IsVisible = !string.IsNullOrWhiteSpace(value);
                _label.HorizontalOptions = string.IsNullOrWhiteSpace(value)
                    ? LayoutOptions.CenterAndExpand
                    : LayoutOptions.End;
                _container.BackgroundColor = string.IsNullOrWhiteSpace(value)
                    ? AppConstants.TableBackgroundColor
                    : AppConstants.WarningColor;
                _labelWarning.Text = value ?? string.Empty;
                _label.TextColor = string.IsNullOrWhiteSpace(value) ? AppConstants.TableSectionColor : Color.White;
            }
        }

        private readonly Label _label;
        private readonly Label _labelWarning;
        private readonly StackLayout _warningView;
        private readonly ContentView _container;

        public InfoFooterComponent() : this(true) { }

        public InfoFooterComponent(bool showOfflineWarning)
        {
            _label = new Label
            {
                FontSize = 12,
                TextColor = AppConstants.TableSectionColor,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            _labelWarning = new Label
            {
                FontSize = 12,
                TextColor = Color.White,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            };

            _warningView = new StackLayout
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                Orientation = StackOrientation.Horizontal,
                Spacing = 3,
                Children =
                {
                    new Image {Source = "warning"},
                    _labelWarning
                }
            };

            _container = new ContentView
            {
                Content = new StackLayout
                {
                    Children =
                    {
                        _warningView,
                        _label
                    },
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Orientation = StackOrientation.Horizontal
                },
                BackgroundColor = AppConstants.TableBackgroundColor,
                Padding = new Thickness(10, 5),
                Margin = new Thickness(0, 0.5, 0, 0),
                VerticalOptions = LayoutOptions.End
            };

            Content = _container;

            BackgroundColor = Color.FromHex("#ccc");
            VerticalOptions = LayoutOptions.End;

            if (!showOfflineWarning)
            {
                Warning = null;
                return;
            }

            Warning = CrossConnectivity.Current.IsConnected ? null : I18N.Offline;
            CrossConnectivity.Current.ConnectivityChanged +=
                (sender, args) => Device.BeginInvokeOnMainThread(() => Warning = args.IsConnected ? null : I18N.Offline);
        }
    }
}