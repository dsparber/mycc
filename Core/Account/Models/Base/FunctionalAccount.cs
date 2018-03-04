using System;
using MyCC.Core.Account.Database;

namespace MyCC.Core.Account.Models.Base
{
    public abstract class FunctionalAccount : Account
    {
        protected readonly AccountDatabase AccountDatabase;
        public DateTime LastUpdate;


        protected FunctionalAccount(int? id, int repositoryId, string name, Money money, DateTime lastUpdate, bool isEnabled = true) : base(id ?? default(int), repositoryId, name, money, isEnabled)
        {
            AccountDatabase = new AccountDatabase();
            LastUpdate = lastUpdate;
        }
    }
}
