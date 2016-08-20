using SQLite;
using models;
using System.Threading.Tasks;
using data.database.interfaces;

namespace data.database.models
{
	[Table("ExchangeRates")]
	public class ExchangeRateDBM : IEntityRepositoryIdDBM<ExchangeRate>
	{
		public ExchangeRateDBM() { }

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public int ReferenceCurrencyId { get; set; }

		public int SecondaryCurrencyId { get; set; }

		public decimal? Rate { get; set; }

		public int RepositoryId { get; set; }

		public async Task<ExchangeRate> Resolve()
		{
			var db = new CurrencyDatabase();
			return new ExchangeRate(await db.Get(ReferenceCurrencyId), await db.Get(SecondaryCurrencyId), Rate);
		}

		public ExchangeRateDBM(ExchangeRate exchangeRate, int repositoryId)
		{
			Id = exchangeRate.Id.Value;
			ReferenceCurrencyId = exchangeRate.ReferenceCurrency.Id.Value;
			SecondaryCurrencyId = exchangeRate.SecondaryCurrency.Id.Value;
			Rate = exchangeRate.Rate;
			RepositoryId = repositoryId;
		}
	}
}

