using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.ExchangeRate.Helpers;
using MyCC.Core.ExchangeRate.Model;
using MyCC.Core.Settings;
using MyCryptos.Forms.Messages;
using Xamarin.Forms;
using AccountDetailView = MyCryptos.Forms.view.pages.AccountDetailView;

namespace MyCryptos.view.components
{
    public class AccountViewCell : CustomViewCell
    {
        readonly INavigation Navigation;

        FunctionalAccount account;
        AccountRepository repository;

        public FunctionalAccount Account
        {
            get { return account; }
            set { account = value; Detail = account.Name; Text = account.Money.ToString(); setTapRecognizer(); }
        }

        public AccountRepository Repository
        {
            get { return repository; }
            set { repository = value; setTapRecognizer(); }
        }

        public AccountViewCell(INavigation navigation)
        {
            Image = "more.png";
            Navigation = navigation;
            setTapRecognizer();
        }

        void setTapRecognizer()
        {
            var gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (sender, e) => Navigation.PushAsync(new AccountDetailView(account));

            if (View != null)
            {
                View.GestureRecognizers.Clear();
                View.GestureRecognizers.Add(gestureRecognizer);
            }
        }

        public override decimal Units { get { return Account.Money.Amount; } }
        public override string Name { get { return Account.Name + Account.Money.Currency.Code; } }
        public override decimal Value
        {
            get
            {
                ExchangeRate rate = null;
                if (Account.Money.Currency.Equals(ApplicationSettings.BaseCurrency))
                {
                    rate = new ExchangeRate(Account.Money.Currency, Account.Money.Currency, 1);
                }
                if (rate == null)
                {
                    rate = ExchangeRateHelper.GetRate(Account.Money.Currency, ApplicationSettings.BaseCurrency);
                }
                return Account.Money.Amount * (rate != null ? rate.RateNotNull : 0);
            }
        }
    }
}


