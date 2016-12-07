using MyCryptos.Core.Account.Repositories.Base;

namespace MyCryptos.Forms.view.addsource
{

    public abstract class AddRepositorySubview : AddSourceSubview
    {
        public abstract OnlineAccountRepository GetRepository(string name);
    }
}
