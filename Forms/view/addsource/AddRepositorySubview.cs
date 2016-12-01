using MyCryptos.Core.Repositories.Account;

namespace MyCryptos.view.addrepositoryviews
{

    public abstract class AddRepositorySubview : AddSourceSubview
    {
        public abstract OnlineAccountRepository GetRepository(string name);
    }
}
