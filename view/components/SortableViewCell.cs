using MyCryptos.models;
using Xamarin.Forms;

namespace MyCryptos.view.components
{
	public abstract class SortableViewCell : ViewCell
	{
		public abstract decimal Units { get; }
		public abstract string Name { get; }
		public abstract decimal Value { get; }
	}
}
