using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.ExchangeRate.Helpers;
using MyCryptos.Core.ExchangeRate.Model;
using MyCryptos.Core.settings;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.view.components;
using Xamarin.Forms;
using AddSourceView = MyCryptos.Forms.view.pages.AddSourceView;

namespace MyCryptos.Forms.view.components
{
	public partial class CoinsListView
	{
		private List<SortableViewCell> cells;

		public CoinsListView()
		{
			InitializeComponent();

			CoinsSection.Title = I18N.Coins;

			Messaging.Loading.SubscribeFinished(this, SetCells);

			Messaging.FetchMissingRates.SubscribeFinished(this, SetCells);
			Messaging.UpdatingAccounts.SubscribeFinished(this, SetCells);
			Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, SetCells);

			Messaging.ReferenceCurrency.SubscribeValueChanged(this, SetCells);
		}

		private static IEnumerable<IGrouping<Currency, Tuple<FunctionalAccount, AccountRepository>>> Groups
		{
			get
			{
				var allAccounts = AccountStorage.Instance.AllElementsWithRepositories;
				return allAccounts.GroupBy(a => a.Item1.Money.Currency);
			}
		}

		private void SetCells()
		{
			var cs = new List<SortableViewCell>();

			foreach (var g in Groups)
			{
				if (g.Key == null) continue;

				var cell = cells?.OfType<CoinViewCell>().ToList().Find(e => g.Key.Equals(e.Currency)) ?? new CoinViewCell(Navigation);
				cell.Accounts = g.ToList();

				var neededRate = new ExchangeRate(cell.Currency, ApplicationSettings.BaseCurrency);
				var rate = ExchangeRateHelper.GetRate(neededRate);
				cell.ExchangeRate = rate;

				cs.Add(cell);
			}
			if (cs.Count == 0)
			{
				var addSourceCell = new CustomViewCell { Text = I18N.AddSource, IsActionCell = true };
				addSourceCell.Tapped += (sender, e) => Navigation.PushOrPushModal(new AddSourceView());
				cs.Add(addSourceCell);
			}

			cells = cs;

			Device.BeginInvokeOnMainThread(() => SortHelper.ApplySortOrder(cells, CoinsSection, Core.Types.SortOrder.Alphabetical, Core.Types.SortDirection.Ascending));
		}
	}
}
