using Android.Support.V4.App;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;

namespace MyCC.Ui.Android.Views.Fragments.AddSource
{
    public abstract class AddSourceFragment : Fragment
    {
        public abstract bool EntryComplete { get; }

        public abstract class Repository : AddSourceFragment
        {
            public abstract OnlineAccountRepository GetRepository();
        }

        public abstract class Account : AddSourceFragment
        {
            public abstract LocalAccount GetAccount();
        }

    }
}