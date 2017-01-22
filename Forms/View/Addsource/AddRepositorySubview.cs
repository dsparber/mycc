using MyCC.Core.Account.Repositories.Base;

namespace MyCC.Forms.view.addsource
{

    public abstract class AddRepositorySubview : AddSourceSubview
    {
        public abstract OnlineAccountRepository GetRepository(string name);
    }
}
