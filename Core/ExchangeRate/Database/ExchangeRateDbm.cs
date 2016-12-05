using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using MyCryptos.Core.Currency.Database;
using SQLite;

namespace MyCryptos.Core.ExchangeRate.Database
{
	[Table("ExchangeRates")]
	public class ExchangeRateDbm : IEntityRepositoryIdDBM<Model.ExchangeRate, string>
	{
		public ExchangeRateDbm() { }

		[PrimaryKey, Column("_id")]
		public string Id
		{
			get
			{
				if (string.Compare(ReferenceCurrencyCode, SecondaryCurrencyCode, System.StringComparison.Ordinal) < 0)
				{
					return ReferenceCurrencyCode + SecondaryCurrencyCode;
				}
				return SecondaryCurrencyCode + ReferenceCurrencyCode;
			}
			set { }
		}

		[Column("ReferenceCode")]
		public string ReferenceCurrencyCode { get; set; }

		[Column("SecondaryCode")]
		public string SecondaryCurrencyCode { get; set; }

		[Column("Rate")]
		public decimal? Rate { get; set; }

		[Column("ExchangeRateRepository")]
		public int ParentId { get; set; }

		public async Task<Model.ExchangeRate> Resolve()
		{
			var db = new CurrencyDatabase();
			return new Model.ExchangeRate(await db.Get(ReferenceCurrencyCode), await db.Get(SecondaryCurrencyCode), Rate) { Id = Id, ParentId = ParentId };
		}

		public ExchangeRateDbm(Model.ExchangeRate exchangeRate)
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
			ParentId = exchangeRate.ParentId;
		}
	}
}

