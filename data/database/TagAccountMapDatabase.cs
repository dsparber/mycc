using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using System.Linq;
using models;
using SQLite;
using Xamarin.Forms;
using data.database.interfaces;
using data.database.helper;
using System;

namespace data.database
{
	public class TagAccountMapDatabase : AbstractDatabase
	{
		public async Task<IEnumerable<TagAccountMapDBM>> GetAll()
		{
			return await (await Connection()).Table<TagAccountMapDBM>().ToListAsync();
		}

		public async Task<IEnumerable<TagAccountMapDBM>> GetForAccountId(int accountId)
		{
			return (await GetAll()).Where(x => x.AccountId == accountId);
		}

		public async Task DeleteWithAccountId(int accountId)
		{
			await Task.WhenAll((await GetForAccountId(accountId)).Select(async t =>
			{
				await (await Connection()).DeleteAsync(t);
			}));
		}

		public async Task Write(Account account, Tag tag)
		{
			var dbObj = new TagAccountMapDBM { TagId = tag.Id.Value, AccountId = account.Id.Value };

			await (await Connection()).InsertAsync(dbObj);
		}

		protected override Task<CreateTablesResult> Create()
		{
			return ConnectionWithoutCreate.CreateTableAsync<TagAccountMapDBM>();
		}
	}
}