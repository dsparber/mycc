using System;
using System.Threading.Tasks;

namespace MyCC.Core.Account.Models.Base
{
    public abstract class OnlineFunctionalAccount : FunctionalAccount
    {

        protected OnlineFunctionalAccount(int? id, int repositoryId, string name, Money money, DateTime lastUpdate, bool isEnabled = true) : base(id ?? default(int), repositoryId, name, money, lastUpdate, isEnabled) { }

        public async Task FetchBalanceOnline()
        {
            await FetchBalanceOnlineTask();
            LastUpdate = DateTime.Now;
        }

        protected abstract Task FetchBalanceOnlineTask();

        public abstract Task FetchTransactionsOnline();
    }
}
