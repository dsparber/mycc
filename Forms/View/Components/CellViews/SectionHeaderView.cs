using MyCC.Forms.Constants;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.CellViews
{
    public class SectionHeaderView : ContentView
    {
        private readonly Label _label;

        public SectionHeaderView(bool showBorder = true)
        {
            _label = new Label
            {
                TextColor = AppConstants.TableSectionColor,
                FontSize = AppConstants.TableSectionFontSize,
                Margin = new Thickness(15, 25, 15, 7.5)
            };
            Content = new ContentView
            {
                Content = new ContentView
                {
                    Content = _label,
                    BackgroundColor = AppConstants.TableBackgroundColor
                },
                BackgroundColor = Color.FromHex("#ccc"),
                Padding = new Thickness(0, 0, 0, showBorder ? 0.5 : 0)
            };
        }

        public string Title
        {
            set { _label.Text = value.ToUpper(); }
        }
    }
}