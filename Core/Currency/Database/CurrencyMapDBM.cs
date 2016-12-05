using System.Threading.Tasks;
using MyCryptos.Core.Database.Interfaces;
using MyCryptos.Core.Models;
using SQLite;

namespace MyCryptos.Core.Database.Models
{
	[Table("CurrencyRepositoryMapElements")]
	public class CurrencyMapDBM : IEntityRepositoryIdDBM<CurrencyMapDBM, string>, PersistableRepositoryElement<string>
	{
		[PrimaryKey, Column("_id")]
		public string Id
		{
			get { return Code + RepositoryId; }
			set { }
		}

		public string Code { get; set; }

		public int RepositoryId { get; set; }

		public Task<CurrencyMapDBM> Resolve()
		{
			return Task.Factory.StartNew(() => this);
		}

		public override bool Equals(object obj)
		{
			if (obj is CurrencyMapDBM)
			{
				var e = (CurrencyMapDBM)obj;
				return Code.Equals(e.Code) && e.Id == Id;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Code.GetHashCode() + Id.GetHashCode();
		}
	}
}

