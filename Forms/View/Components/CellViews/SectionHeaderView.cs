using MyCC.Forms.Constants;
using Xamarin.Forms;

namespace MyCC.Forms.view.components.CellViews
{
    public class SectionHeaderView : ContentView
    {
        private readonly Label _label;

        public SectionHeaderView()
        {
            _label = new Label { TextColor = AppConstants.TableSectionColor, FontSize = AppConstants.TableSectionFontSize, Margin = new Thickness(15, 24, 8, 15) };
            Content = _label;
        }

        public string Title
        {
            set { _label.Text = value.ToUpper(); }
        }
    }
}