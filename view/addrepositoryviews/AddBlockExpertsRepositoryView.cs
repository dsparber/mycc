using data.repositories.account;
using Xamarin.Forms;
using MyCryptos.view.components;
using MyCryptos.resources;
using data.repositories.currency;
using MyCryptos.models;

namespace MyCryptos.view.addrepositoryviews
{
    public sealed class AddBlockExpertsRepositoryView : AddAddressAndCoinRepositoryView
    {
        public AddBlockExpertsRepositoryView(INavigation navigation) : base(navigation, typeof(BlockExpertsCurrencyRepository), I18N.BlockExperts) { }
        
        protected override OnlineAccountRepository GetRepository(string name, Currency coin, string address)
        {
            return new BlockExpertsAccountRepository(name, coin, address);
        }
    }
}
