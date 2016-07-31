using SQLite;
using models;

namespace data.database.models
{
	public class ExchangeRateDBM
	{
		public ExchangeRateDBM() {}

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public int ReferenceCurrencyId { get; set; }

		public int SecondaryCurrencyId { get; set; }

		public decimal? Rate { get; set; }

		public ExchangeRate ToExchangeRate(Currency referenceCurrency, Currency secondaryCurrency)
		{
			return new ExchangeRate(referenceCurrency, secondaryCurrency, Rate);
		}

		public ExchangeRateDBM(ExchangeRate exchangeRate)
		{
			Id = exchangeRate.Id.Value;
			ReferenceCurrencyId = exchangeRate.ReferenceCurrency.Id.Value;
			SecondaryCurrencyId = exchangeRate.SecondaryCurrency.Id.Value;
			Rate = exchangeRate.Rate;
		}
	}
}

