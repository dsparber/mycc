using MyCryptos.Core.Models;

namespace MyCryptos.view.addrepositoryviews
{

    public abstract class AddAccountSubview : AddSourceSubview
    {
        public abstract Account GetAccount(string name);
    }
}
