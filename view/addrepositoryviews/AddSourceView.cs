using System;
using data.repositories.account;
using Xamarin.Forms;

namespace MyCryptos.view.addrepositoryviews
{

	public abstract class AddSourceView
	{
		public abstract TableSection InputSection { get; }
		public abstract bool Enabled { set; }
		public abstract string Description { get; }
		public abstract string DefaultName { get; }
		public Action NameChanged = () => { };
		public abstract OnlineAccountRepository GetRepository(string name);
		public abstract void Unfocus();
	}
}
