using System;
using System.Threading.Tasks;

namespace MyCC.Core.Account.Models.Base
{
    public abstract class OnlineFunctionalAccount : FunctionalAccount
    {
        public DateTime LastUpdate;

        protected OnlineFunctionalAccount(int? id, int repositoryId, string name, Money money, bool isEnabled = true) : base(id ?? default(int), repositoryId, name, money, isEnabled)
        {
            LastUpdate = DateTime.MinValue;
        }

        public abstract Task FetchBalanceOnline();
        public abstract Task FetchTransactionsOnline();
    }
}
