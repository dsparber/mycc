using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Currency.Repositories;
using MyCryptos.Core.Currency.Storage;
using MyCryptos.Core.ExchangeRate.Database;
using MyCryptos.Core.ExchangeRate.Storage;

namespace MyCryptos.Core.ExchangeRate.Repositories
{
    public class BittrexAvailableRatesRepository : AvailableRatesRepository
    {
        List<Model.ExchangeRate> Elements;

        public BittrexAvailableRatesRepository(string name) : base(AvailableRatesRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY, name)
        {
            Elements = new List<Model.ExchangeRate>();
        }

        public override bool IsAvailable(Model.ExchangeRate element)
        {
            return Elements.Contains(element);
        }

        public override Task<bool> FetchOnline()
        {
            return Task.Factory.StartNew(() =>
            {
                var repository = CurrencyStorage.Instance.RepositoryOfType<BittrexCurrencyRepository>();
                var codes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.ParentId == repository.Id).Select(e => e.Code);

                Elements = CurrencyStorage.Instance.AllElements.Where(e => codes.Contains(e?.Code)).Select(e => new Model.ExchangeRate(Currency.Model.Currency.BTC, e)).ToList();
                return true;
            });
        }

        public override ExchangeRateRepository ExchangeRateRepository
        {
            get
            {
                return ExchangeRateStorage.Instance.Repositories.OfType<BittrexExchangeRateRepository>().FirstOrDefault();
            }
        }

        public override Model.ExchangeRate ExchangeRateWithCurrency(Currency.Model.Currency currency)
        {
            return Elements.ToList().Find(e => e.Contains(currency));
        }

        public override List<Model.ExchangeRate> ExchangeRatesWithCurrency(Currency.Model.Currency currency)
        {
            return Elements.Where(e => e.Contains(currency)).ToList();
        }
    }
}

