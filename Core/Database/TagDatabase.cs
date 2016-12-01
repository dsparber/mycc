using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Helper;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Models;
using SQLite;

namespace MyCryptos.Core.Database
{
    public class TagDatabase : AbstractDatabase<TagDBM, Tag, int>
    {
        public override async Task<IEnumerable<TagDBM>> GetAllDbObjects()
        {
            return await (await Connection).Table<TagDBM>().ToListAsync();
        }

        protected override async Task Create(SQLiteAsyncConnection connection)
        {
            await connection.CreateTableAsync<TagDBM>();
        }

        public async override Task<TagDBM> GetDbObject(int id)
        {
            return await (await Connection).FindAsync<TagDBM>(p => p.Id == id);
        }

        protected override TagDBM Resolve(Tag element)
        {
            return new TagDBM(element);
        }
    }
}