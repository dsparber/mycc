using System.Threading.Tasks;
using MyCryptos.Core.Database.Interfaces;
using MyCryptos.Core.Models;
using SQLite;

namespace MyCryptos.Core.Database.Models
{
    [Table("ExchangeRates")]
    public class ExchangeRateDBM : IEntityRepositoryIdDBM<ExchangeRate, string>
    {
        public ExchangeRateDBM() { }

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

        public string ReferenceCurrencyCode { get; set; }

        public string SecondaryCurrencyCode { get; set; }

        public decimal? Rate { get; set; }

        public int RepositoryId { get; set; }

        public async Task<ExchangeRate> Resolve()
        {
            var db = new CurrencyDatabase();
            return new ExchangeRate(await db.Get(ReferenceCurrencyCode), await db.Get(SecondaryCurrencyCode), Rate) { Id = Id, RepositoryId = RepositoryId };
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

