using data.repositories.account;
using MyCryptos.data.repositories.account;

namespace MyCryptos.view.addrepositoryviews
{
    public sealed class AddEtherchainRepositoryView : AddAddressRepositoryView
    {
        protected override OnlineAccountRepository repository
        {
            get { return new EthereumAccountRepository(null); }
        }

        protected override OnlineAccountRepository GetRepository(string name, string address)
        {
            return new EthereumAccountRepository(name, address);
        }
    }
}
