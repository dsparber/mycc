using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using MyCryptos.Core.Account.Repositories.Base;
using SQLite;

namespace MyCryptos.Core.Account.Database
{
    public class AccountRepositoryDatabase : AbstractDatabase<AccountRepositoryDbm, AccountRepository, int>
    {
        public override async Task<IEnumerable<AccountRepositoryDbm>> GetAllDbObjects()
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

