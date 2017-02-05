using Xamarin.Forms;

namespace MyCC.Forms.View.Components.Cells
{
    public abstract class SortableViewCell : ViewCell
    {
        public abstract decimal Units { get; }
        public abstract string Name { get; }
        public abstract decimal Value { get; }
    }
}
