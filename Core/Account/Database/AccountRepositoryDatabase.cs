using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Account.Repositories.Base;
using SQLite;

namespace MyCC.Core.Account.Database
{
    public class AccountRepositoryDatabase : AbstractDatabase<AccountRepositoryDbm, AccountRepository, int>
    {
        protected override async Task<IEnumerable<AccountRepositoryDbm>> GetAllDbObjects()
        {
            return await (await Connection).Table<AccountRepositoryDbm>().ToListAsync();
        }

        public override async Task<AccountRepositoryDbm> GetDbObject(int id)
        {
            return await (await Connection).FindAsync<AccountRepositoryDbm>(p => p.Id == id);
        }

        protected override Task Create(SQLiteAsyncConnection connection)
        {
            return connection.CreateTableAsync<AccountRepositoryDbm>();

        }

        protected override AccountRepositoryDbm Resolve(AccountRepository element)
        {
            return new AccountRepositoryDbm(element);
        }
    }
}

