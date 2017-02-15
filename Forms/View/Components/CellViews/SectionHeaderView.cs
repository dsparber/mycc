using MyCC.Forms.Constants;
using Xamarin.Forms;

namespace MyCC.Forms.view.components.CellViews
{
    public class SectionHeaderView : ContentView
    {
        private readonly Label _label;

        public SectionHeaderView()
        {
            _label = new Label { TextColor = AppConstants.TableSectionColor, FontSize = AppConstants.TableSectionFontSize, Margin = new Thickness(15, 25, 15, 7.5) };
            Content = new ContentView { Content = new ContentView { Content = _label, BackgroundColor = AppConstants.TableBackgroundColor }, BackgroundColor = Color.FromHex("#ccc"), Padding = new Thickness(0, 0, 0, 0.5) };
        }

        public string Title
        {
            set { _label.Text = value.ToUpper(); }
        }
    }
}