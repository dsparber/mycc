using MyCC.Forms.Constants;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components
{
    public class InfoFooterComponent : ContentView
    {
        public string Text { set { _label.Text = value; } }

        private readonly Label _label;

        public InfoFooterComponent()
        {
            _label = new Label { FontSize = 12, TextColor = AppConstants.TableSectionColor, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.End };

            Content = new ContentView
            {
                Content = _label,
                BackgroundColor = AppConstants.TableBackgroundColor,
                Padding = new Thickness(5),
                Margin = new Thickness(0, 0.5, 0, 0),
                VerticalOptions = LayoutOptions.End
            };
            BackgroundColor = Color.FromHex("#ccc");
            VerticalOptions = LayoutOptions.End;
        }
    }
}