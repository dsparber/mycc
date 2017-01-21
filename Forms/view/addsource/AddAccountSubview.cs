using MyCC.Core.Account.Models.Base;

namespace MyCryptos.Forms.view.addsource
{

    public abstract class AddAccountSubview : AddSourceSubview
    {
        public abstract FunctionalAccount GetAccount(string name);
    }
}
