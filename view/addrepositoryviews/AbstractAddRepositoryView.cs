using data.repositories.account;
using Xamarin.Forms;

namespace MyCryptos.view.addrepositoryviews
{

	public abstract class AbstractAddRepositoryView
	{
		public abstract TableSection InputSection { get; }
		public abstract bool Enabled { set; }
		public abstract string Description { get; }
		public abstract string DefaultName { get; }
		public abstract AccountRepository GetRepository(string name);
	}
}
