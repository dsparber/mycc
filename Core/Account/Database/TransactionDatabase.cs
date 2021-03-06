using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Account.Models.Base;
using SQLite;

namespace MyCC.Core.Account.Database
{
    public class TransactionDatabase : AbstractDatabase<TransactionDbm, Transaction, string>
    {
        protected override async Task<IEnumerable<TransactionDbm>> GetAllDbObjects()
        {
            return await (await Connection).Table<TransactionDbm>().ToListAsync();
        }

        protected override async Task Create(SQLiteAsyncConnection connection)
        {
            await connection.CreateTableAsync<TransactionDbm>();
        }

        protected override async Task<TransactionDbm> GetDbObject(string id)
        {
            return await (await Connection).FindAsync<TransactionDbm>(p => p.Id.Equals(id));
        }

        protected override TransactionDbm Resolve(Transaction element)
        {
            return new TransactionDbm(element);
        }
    }
}