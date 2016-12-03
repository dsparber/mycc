using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MyCryptos.Core.Helpers;
using MyCryptos.Core.Models;
using MyCryptos.Core.Repositories.Account;
using MyCryptos.Core.Settings;
using MyCryptos.Core.Storage;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.view.components;
using view;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.components
{
	public partial class CoinsTableView
	{
		private List<SortableViewCell> cells;

		public CoinsTableView()
		{
			InitializeComponent();

			CoinsSection.Title = I18N.Coins;

			Messaging.Loading.SubscribeFinished(this, SetCells);
			Messaging.UpdatingExchangeRates.SubscribeFinished(this, SetCells);

			Messaging.ReferenceCurrency.SubscribeValueChanged(this, SetCells);
			Messaging.UpdatingAccounts.SubscribeFinished(this, SetCells);
			Messaging.SortOrder.SubscribeValueChanged(this, () => SortHelper.ApplySortOrder(cells, CoinsSection));
		}

		IEnumerable<IGrouping<Currency, Tuple<Account, AccountRepository>>> groups
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

			foreach (var g in groups)
			{
				if (g.Key == null) continue;

				var cell = cells?.OfType<CoinViewCell>().ToList().Find(e => g.Key.Equals(e.Currency));
				if (cell == null)
				{
					cell = new CoinViewCell(Navigation) { Accounts = g.ToList(), IsLoading = true };
				}
				else
				{
					cell.Accounts = g.ToList();
				}

				var neededRate = new ExchangeRate(cell.Currency, ApplicationSettings.BaseCurrency);
				var rate = ExchangeRateHelper.GetRate(neededRate);
				cell.ExchangeRate = rate;

				cell.IsLoading = rate != null && !rate.Rate.HasValue;

				cs.Add(cell);
			}
			if (cs.Count == 0)
			{
				var addSourceCell = new CustomViewCell { Text = I18N.AddSource, IsActionCell = true };
				addSourceCell.Tapped += (sender, e) => Navigation.PushOrPushModal(new AddSourceView());
				cs.Add(addSourceCell);
			}

			cells = cs;

			Device.BeginInvokeOnMainThread(() => SortHelper.ApplySortOrder(cells, CoinsSection));
		}
	}
}
