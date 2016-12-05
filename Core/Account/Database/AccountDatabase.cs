using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using MyCryptos.Core.Account.Models.Base;
using SQLite;

namespace MyCryptos.Core.Account.Database
{
    public class AccountDatabase : AbstractDatabase<AccountDBM, FunctionalAccount, int>
    {
        public override async Task<IEnumerable<AccountDBM>> GetAllDbObjects()
        {
            return await (await Connection).Table<AccountDBM>().ToListAsync();
        }

        protected override async Task Create(SQLiteAsyncConnection connection)
        {
            await connection.CreateTableAsync<AccountDBM>();
        }

        public async override Task<AccountDBM> GetDbObject(int id)
        {
            return await (await Connection).FindAsync<AccountDBM>(p => p.Id == id);
        }

        protected override AccountDBM Resolve(FunctionalAccount element)
        {
            return new AccountDBM(element);
        }
    }
}