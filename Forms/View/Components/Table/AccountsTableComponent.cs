using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using MyCC.Core.Account.Storage;
using MyCC.Core.Helpers;
using MyCC.Forms.Constants;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.BaseComponents;
using MyCC.Forms.View.Components.CellViews;
using MyCC.Forms.View.Pages;
using MyCC.Ui;
using MyCC.Ui.DataItems;
using MyCC.Ui.Messages;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.Table
{
    public class AccountsTableComponent : ContentView
    {
        private readonly HybridWebView _webView;
        private readonly string _currencyId;
        private readonly bool _useEnabledAccounts;
        private static bool _firstCall = true;

        private readonly Dictionary<int, Action> _headerClickCallbacks;
        private static int _currentId;

        public AccountsTableComponent(INavigation navigation, string currencyId, bool useEnabledAccounts)
        {
            _currencyId = currencyId;
            _useEnabledAccounts = useEnabledAccounts;
            _headerClickCallbacks = new Dictionary<int, Action>();

            _webView = new HybridWebView("Html/accountsTable.html")
            {
                LoadFinished = async () =>
                {
                    UpdateView();
                    if (!_firstCall) return;

                    await Task.Delay(1000);
                    UpdateView();
                    _firstCall = false;
                }
            };

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
            _webView.RegisterCallback("HeaderClickedCallback", id => _headerClickCallbacks[int.Parse(id)].Invoke());

            var stack = new StackLayout { Spacing = 0, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = AppConstants.TableBackgroundColor };

            stack.Children.Add(new SectionHeaderView(false) { Title = _useEnabledAccounts ? I18N.Accounts : I18N.DisabledAccounts });
            stack.Children.Add(_webView);

            Content = stack;

            UpdateView();

            Messaging.Update.Balances.Subscribe(this, UpdateView);
            Messaging.Modified.Balances.Subscribe(this, UpdateView);
            Messaging.Sort.Accounts.Subscribe(this, UpdateView);
            Messaging.Update.Rates.Subscribe(this, UpdateView);
        }

        private void UpdateView()
        {
            try
            {
                var items = (_useEnabledAccounts ? UiUtils.Get.AccountsGroup.EnabledAccountsItems(_currencyId) : UiUtils.Get.AccountsGroup.DisabledAccountsItems(_currencyId)).ToList();
                if (!items.Any()) return;

                Device.BeginInvokeOnMainThread(() =>
                {
                    _headerClickCallbacks.Clear();
                    _currentId = 0;
                    _webView.CallJsFunction("setHeader", UiUtils.Get.AccountsGroup.SortButtonsAccounts.Select(button => new HeaderData(button, _currentId += 1, this)), string.Empty);
                    _webView.CallJsFunction("updateTable", items.Select(item => new Data(item)).ToArray(), string.Empty);
                });
            }
            catch (Exception e)
            {
                e.LogError();
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

            public Data(AccountItem account)
            {
                Name = account.Name;
                Amount = account.FormattedValue;
                Id = account.Id;
                Disabled = !account.Enabled;
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
            public readonly int Id;
            [DataMember]
            public readonly bool? Ascending;

            public HeaderData(SortButtonItem sortButtonItem, int id, AccountsTableComponent parent)
            {
                Text = sortButtonItem.Text;
                Id = id;
                Ascending = sortButtonItem.SortAscending;
                parent._headerClickCallbacks[id] = sortButtonItem.OnClick;

            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            UpdateView();
        }
    }
}
