using MyCryptos.data.repositories.account;
using Xamarin.Forms;
using MyCryptos.resources;
using MyCryptos.models;
using MyCryptos.data.repositories.currency;
using data.repositories.account;

namespace MyCryptos.view.addrepositoryviews
{
    public sealed class AddCryptoIdRepositoryView : AddAddressAndCoinRepositoryView
    {

        public AddCryptoIdRepositoryView(INavigation navigation) : base(navigation, typeof(CryptoIdCurrencyRepository), I18N.CryptoId) { }

        protected override OnlineAccountRepository GetRepository(string name, Currency coin, string address)
        {
            return new CryptoIdAccountRepository(name, coin, address);
        }
    }
}
