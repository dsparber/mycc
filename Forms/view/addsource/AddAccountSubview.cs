using MyCryptos.Core.Account.Models;
using MyCryptos.Core.Account.Models.Base;

namespace MyCryptos.view.addrepositoryviews
{

    public abstract class AddAccountSubview : AddSourceSubview
    {
        public abstract FunctionalAccount GetAccount(string name);
    }
}
