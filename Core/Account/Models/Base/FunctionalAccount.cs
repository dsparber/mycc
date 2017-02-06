using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Database;

namespace MyCC.Core.Account.Models.Base
{
    public abstract class FunctionalAccount : Account
    {
        protected readonly AccountDatabase AccountDatabase;
        protected readonly TransactionDatabase TransactionDatabase;
        public DateTime LastUpdate;


        public List<Transaction> Transactions { get; private set; }

        protected FunctionalAccount(int? id, int repositoryId, string name, Money money, DateTime lastUpdate, bool isEnabled = true) : base(id ?? default(int), repositoryId, name, money, isEnabled)
        {
            Transactions = new List<Transaction>();

            AccountDatabase = new AccountDatabase();
            TransactionDatabase = new TransactionDatabase();
            LastUpdate = lastUpdate;
        }

        public async Task LoadBalanceFromDatabase()
        {
            Money = (await AccountDatabase.Get(Id)).Money;
        }
        public async Task LoadTransactionsFromDatabase()
        {
            Transactions = (await TransactionDatabase.Get((TransactionDbm t) => t.ParentId == Id)).ToList();
        }
    }
}
