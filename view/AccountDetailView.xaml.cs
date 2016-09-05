using System;
using System.Collections.Generic;
using data.repositories.account;
using models;
using MyCryptos.resources;
using Xamarin.Forms;

namespace view
{
	public partial class AccountDetailView : ContentPage
	{
		public AccountDetailView(Account account, AccountRepository repository)
		{
			InitializeComponent();

			Title = account.Name;
			Header.TitleText = account.Money.ToString();
			Header.InfoText = string.Format(InternationalisationResources.SourceText, repository.Name);
		}
	}
}

