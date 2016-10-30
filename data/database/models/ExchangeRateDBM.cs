using SQLite;
using System.Threading.Tasks;
using data.database.interfaces;
using MyCryptos.models;

namespace data.database.models
{
	[Table("ExchangeRates")]
	public class ExchangeRateDBM : IEntityRepositoryIdDBM<ExchangeRate, string>
	{
		public ExchangeRateDBM() { }

		[PrimaryKey, Column("_id")]
		public string Id
		{
			get { return ReferenceCurrencyCode + SecondaryCurrencyCode; }
			set { }
		}

		[MaxLength(3)]
		public string ReferenceCurrencyCode { get; set; }

		[MaxLength(3)]
		public string SecondaryCurrencyCode { get; set; }

		public decimal? Rate { get; set; }

		public int RepositoryId { get; set; }

		public async Task<ExchangeRate> Resolve()
		{
			var db = new CurrencyDatabase();
			return new ExchangeRate((await db.Get(ReferenceCurrencyCode)) ?? new Currency(ReferenceCurrencyCode), (await db.Get(SecondaryCurrencyCode)) ?? new Currency(SecondaryCurrencyCode), Rate) { Id = Id, RepositoryId = RepositoryId };
		}

		public ExchangeRateDBM(ExchangeRate exchangeRate)
		{

			Id = exchangeRate.Id;

			if (exchangeRate.ReferenceCurrency != null)
			{
				ReferenceCurrencyCode = exchangeRate.ReferenceCurrency.Code;
			}
			if (exchangeRate.SecondaryCurrency != null)
			{
				SecondaryCurrencyCode = exchangeRate.SecondaryCurrency.Code;
			}

			Rate = exchangeRate.Rate;
			RepositoryId = exchangeRate.RepositoryId;
		}
	}
}

