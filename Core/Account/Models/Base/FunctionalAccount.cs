using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Database;

namespace MyCC.Core.Account.Models.Base
{
    public abstract class FunctionalAccount : Account
    {
        protected readonly AccountDatabase accountDatabase;
        protected readonly TransactionDatabase transactionDatabase;

        public List<Transaction> Transactions { get; private set; }

        protected FunctionalAccount(int? id, int repositoryId, string name, Money money) : base(id ?? default(int), repositoryId, name, money)
        {
            Transactions = new List<Transaction>();

            accountDatabase = new AccountDatabase();
            transactionDatabase = new TransactionDatabase();
        }

        public async Task LoadBalanceFromDatabase()
        {
            Money = (await accountDatabase.Get(Id)).Money;
        }
        public async Task LoadTransactionsFromDatabase()
        {
            Transactions = (await transactionDatabase.Get((TransactionDbm t) => t.ParentId == Id)).ToList();
        }
    }
}
