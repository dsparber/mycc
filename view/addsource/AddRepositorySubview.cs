using System;
using data.repositories.account;
using Xamarin.Forms;

namespace MyCryptos.view.addrepositoryviews
{

	public abstract class AddRepositorySubview : AddSourceSubview
	{
		public abstract OnlineAccountRepository GetRepository(string name);
	}
}
