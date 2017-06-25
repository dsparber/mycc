using MyCC.Core.Account.Models.Implementations;

namespace MyCC.Forms.View.Addsource
{

    public abstract class AddAccountSubview : AddSourceSubview
    {
        public abstract LocalAccount GetAccount(string name);
    }
}
