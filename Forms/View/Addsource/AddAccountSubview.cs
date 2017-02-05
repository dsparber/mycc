using MyCC.Core.Account.Models.Base;

namespace MyCC.Forms.View.Addsource
{

    public abstract class AddAccountSubview : AddSourceSubview
    {
        public abstract FunctionalAccount GetAccount(string name);
    }
}
