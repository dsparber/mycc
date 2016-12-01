using MyCryptos.view.components;
using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Constants;
using MyCryptos.Core.Helpers;
using MyCryptos.Core.Models;
using MyCryptos.Core.Repositories.Account;
using MyCryptos.Core.Settings;
using MyCryptos.Core.Storage;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Resources;
using view;
using Xamarin.Forms;

namespace MyCryptos.view
{
    public partial class CoinsTableView : ContentView
    {
        List<SortableViewCell> Cells;

        public CoinsTableView()
        {
            InitializeComponent();

            Cells = new List<SortableViewCell>();
            CoinsSection.Title = I18N.Coins;

            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedExchangeRates, str => updateView());
            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrency, str => updateView());
            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedAccounts, str => updateView());
            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedSortOrder, str => SortHelper.ApplySortOrder(Cells, CoinsSection));
        }

        void updateView()
        {
            setCells();
        }

        IEnumerable<IGrouping<Currency, Tuple<Account, AccountRepository>>> groups
        {
            get
            {
                var allAccounts = AccountStorage.Instance.AllElementsWithRepositories;
                return allAccounts.GroupBy(a => a.Item1.Money.Currency);
            }
        }

        void setCells()
        {
            var cells = new List<SortableViewCell>();

            foreach (var g in groups)
            {
                if (g.Key != null)
                {
                    var cell = Cells.OfType<CoinViewCell>().ToList().Find(e => g.Key.Equals(e.Currency));
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

                    cells.Add(cell);
                }
            }
            if (cells.Count == 0)
            {
                var addSourceCell = new CustomViewCell { Text = I18N.AddSource, IsActionCell = true };
                addSourceCell.Tapped += (sender, e) => Navigation.PushOrPushModal(new AddSourceView());
                cells.Add(addSourceCell);
            }

            Cells = cells;
            SortHelper.ApplySortOrder(Cells, CoinsSection);

        }
    }
}
