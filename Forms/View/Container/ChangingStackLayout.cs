using MyCC.Forms.Constants;
using MyCC.Forms.Messages;
using Xamarin.Forms;

namespace MyCC.Forms.View.Container
{
    public class ChangingStackLayout : StackLayout
    {
        public ChangingStackLayout()
        {
            Spacing = 0;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.FillAndExpand;
            BackgroundColor = AppConstants.TableBackgroundColor;
            SetChildLayout();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            var orientation = width > height ? StackOrientation.Horizontal : StackOrientation.Vertical;
            if (orientation == Orientation) return;

            Orientation = orientation;
            Messaging.Layout.SendValueChanged();
        }

        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);
            SetChildLayout();
        }

        private void SetChildLayout()
        {
            if (Children.Count > 1)
            {
                Children[1].HorizontalOptions = LayoutOptions.FillAndExpand;
            }
        }
    }
}