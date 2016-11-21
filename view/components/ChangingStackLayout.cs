using Xamarin.Forms;

namespace MyCryptos.view.components
{
    public class ChangingStackLayout : StackLayout
    {
        public ChangingStackLayout()
        {
            Spacing = 0;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            Orientation = width > height ? StackOrientation.Horizontal : StackOrientation.Vertical;
        }
    }
}