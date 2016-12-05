using MyCryptos.Core.Account.Models;

namespace MyCryptos.view.addrepositoryviews
{

	public abstract class AddAccountSubview : AddSourceSubview
	{
		public abstract FunctionalAccount GetAccount(string name);
	}
}
