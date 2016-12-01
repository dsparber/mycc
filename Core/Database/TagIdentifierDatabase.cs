using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Helper;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Models;
using SQLite;

namespace MyCryptos.Core.Database
{
    public class TagIdentifierDatabase : AbstractDatabase<TagIdentifierDBM, TagIdentifier, int>
    {
        public override async Task<IEnumerable<TagIdentifierDBM>> GetAllDbObjects()
        {
            return await (await Connection).Table<TagIdentifierDBM>().ToListAsync();
        }

        protected override async Task Create(SQLiteAsyncConnection connection)
        {
            await connection.CreateTableAsync<TagIdentifierDBM>();
        }

        public async override Task<TagIdentifierDBM> GetDbObject(int id)
        {
            return await (await Connection).FindAsync<TagIdentifierDBM>(p => p.Id == id);
        }

        protected override TagIdentifierDBM Resolve(TagIdentifier element)
        {
            return new TagIdentifierDBM(element);
        }
    }
}