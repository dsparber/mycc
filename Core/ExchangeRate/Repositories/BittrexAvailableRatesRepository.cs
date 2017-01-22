using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currency.Repositories;
using MyCC.Core.Currency.Storage;
using MyCC.Core.ExchangeRate.Database;
using MyCC.Core.ExchangeRate.Storage;

namespace MyCC.Core.ExchangeRate.Repositories
{
    public class BittrexAvailableRatesRepository : AvailableRatesRepository
    {
        private List<Model.ExchangeRate> Elements;

        public BittrexAvailableRatesRepository(int id) : base(id)
        {
            Elements = new List<Model.ExchangeRate>();
        }
        public override int RepositoryTypeId => AvailableRatesRepositoryDbm.DB_TYPE_BITTREX_REPOSITORY;

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

                Elements = CurrencyStorage.Instance.AllElements.Where(e => codes.Contains(e?.Code)).Select(e => new Model.ExchangeRate(Currency.Model.Currency.Btc, e)).ToList();
                return true;
            });
        }

        public override ExchangeRateRepository ExchangeRateRepository => ExchangeRateStorage.Instance.Repositories.OfType<BittrexExchangeRateRepository>().FirstOrDefault();

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

