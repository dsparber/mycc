using MyCryptos.Core.Account.Repositories.Base;

namespace MyCryptos.view.addrepositoryviews
{

    public abstract class AddRepositorySubview : AddSourceSubview
    {
        public abstract OnlineAccountRepository GetRepository(string name);
    }
}
