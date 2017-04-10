using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.Resources;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Forms.Constants;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.BaseComponents;
using MyCC.Forms.View.Components.CellViews;
using MyCC.Forms.View.Pages;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.Table
{
    public class AccountsTableComponent : ContentView
    {
        private readonly HybridWebView _webView;
        private readonly Currency _currency;
        private readonly bool _useEnabledAccounts;

        public AccountsTableComponent(INavigation navigation, Currency currency, bool useEnabledAccounts)
        {
            _currency = currency;
            _useEnabledAccounts = useEnabledAccounts;

            _webView = new HybridWebView("Html/accountsTable.html") { LoadFinished = UpdateView };

            _webView.RegisterCallback("Callback", idString =>
            {
                var id = int.Parse(idString);
                var account = AccountStorage.Instance.AllElements.Find(a => a.Id.Equals(id));

                Device.BeginInvokeOnMainThread(() => navigation.PushAsync(new AccountView(account)));
            });

            _webView.RegisterCallback("CallbackSizeAllocated", sizeString =>
            {
                var size = int.Parse(sizeString);
                Device.BeginInvokeOnMainThread(() => _webView.HeightRequest = size);
            });

            _webView.RegisterCallback("HeaderClickedCallback", type =>
            {
                SortOrder value;
                var clickedSortOrder = Enum.TryParse(type, out value) ? value : SortOrder.None;
                if (clickedSortOrder == ApplicationSettings.SortOrderAccounts)
                {
                    ApplicationSettings.SortDirectionAccounts = ApplicationSettings.SortDirectionAccounts == SortDirection.Ascending
                        ? SortDirection.Descending
                        : SortDirection.Ascending;
                }
                ApplicationSettings.SortOrderAccounts = clickedSortOrder;

                UpdateView();
            });

            var stack = new StackLayout { Spacing = 0, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = AppConstants.TableBackgroundColor };

            stack.Children.Add(new SectionHeaderView(false) { Title = _useEnabledAccounts ? I18N.Accounts : I18N.DisabledAccounts });
            stack.Children.Add(_webView);

            Content = stack;

            UpdateView();

            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingAccounts.SubscribeFinished(this, UpdateView);
        }

        private void UpdateView()
        {
            try
            {
                var items = AccountStorage.AccountsWithCurrency(_currency).Where(a => a.IsEnabled == _useEnabledAccounts).Select(a => new Data(a)).ToList();

                if (!items.Any()) return;

                Func<Data, object> sortLambda;
                switch (ApplicationSettings.SortOrderAccounts)
                {
                    case SortOrder.Alphabetical: sortLambda = d => d.Name; break;
                    case SortOrder.ByUnits: sortLambda = d => decimal.Parse(d.Amount.Replace("<", "").Trim()); break;
                    case SortOrder.ByValue: sortLambda = d => decimal.Parse(d.Amount.Replace("<", "").Trim()); break;
                    case SortOrder.None: sortLambda = d => 1; break;
                    default: sortLambda = d => 1; break;
                }

                items = ApplicationSettings.SortDirectionAccounts == SortDirection.Ascending ? items.OrderBy(sortLambda).ToList() : items.OrderByDescending(sortLambda).ToList();

                Device.BeginInvokeOnMainThread(() =>
                {
                    _webView.CallJsFunction("setHeader", new[]{
                    new HeaderData(I18N.Name, SortOrder.Alphabetical.ToString()),
                    new HeaderData(I18N.Amount, SortOrder.ByUnits.ToString())
                        }, string.Empty);
                    _webView.CallJsFunction("updateTable", items.ToArray(), new SortData(), DependencyService.Get<ILocalise>().GetCurrentCultureInfo().Name);

                    if (Device.RuntimePlatform.Equals(Device.Android))
                    {
                        _webView.HeightRequest = 38 * (items.Count + 1) + 1;
                    }
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        [DataContract]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        public class Data
        {
            [DataMember]
            public readonly string Name;
            [DataMember]
            public readonly string Amount;
            [DataMember]
            public readonly bool Disabled;
            [DataMember]
            public readonly int Id;

            public Data(Account account)
            {
                Name = account.Name;
                Amount = account.Money.ToString8Digits(false);
                Id = account.Id;
                Disabled = !account.IsEnabled;
            }
        }

        [DataContract]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        public class SortData
        {
            [DataMember]
            public readonly string Direction;
            [DataMember]
            public readonly string Type;

            public SortData()
            {
                Direction = ApplicationSettings.SortDirectionAccounts.ToString();
                Type = ApplicationSettings.SortOrderAccounts.ToString();
            }

        }

        [DataContract]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        public class HeaderData
        {
            [DataMember]
            public readonly string Text;
            [DataMember]
            public readonly string Type;

            public HeaderData(string text, string type)
            {
                Text = text;
                Type = type;
            }

        }
    }
}
