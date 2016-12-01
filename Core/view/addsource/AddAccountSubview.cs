using System;
using data.repositories.account;
using MyCryptos.models;
using Xamarin.Forms;

namespace MyCryptos.view.addrepositoryviews
{

	public abstract class AddAccountSubview : AddSourceSubview
	{
		public abstract Account GetAccount(string name);
	}
}
