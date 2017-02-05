using MyCC.Core.Account.Repositories.Base;

namespace MyCC.Forms.View.Addsource
{

    public abstract class AddRepositorySubview : AddSourceSubview
    {
        public abstract OnlineAccountRepository GetRepository(string name);
    }
}
