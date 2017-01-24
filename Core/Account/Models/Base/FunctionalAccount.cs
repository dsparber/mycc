﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Database;

namespace MyCC.Core.Account.Models.Base
{
    public abstract class FunctionalAccount : Account
    {
        protected readonly AccountDatabase AccountDatabase;
        protected readonly TransactionDatabase TransactionDatabase;

        public List<Transaction> Transactions { get; private set; }

        protected FunctionalAccount(int? id, int repositoryId, string name, Money money) : base(id ?? default(int), repositoryId, name, money)
        {
            Transactions = new List<Transaction>();

            AccountDatabase = new AccountDatabase();
            TransactionDatabase = new TransactionDatabase();
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
