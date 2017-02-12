using MyCC.Forms.Constants;
using Xamarin.Forms;

namespace MyCC.Forms.view.components.CellViews
{
    public class SectionFooterView : ContentView
    {
        private readonly Label _label;

        public SectionFooterView()
        {
            _label = new Label { TextColor = AppConstants.TableSectionColor, FontSize = AppConstants.TableSectionFontSize, Margin = new Thickness(15, 8, 15, 24) };
            Content = _label;
        }

        public string Text
        {
            set { _label.Text = value; }
        }
    }
}