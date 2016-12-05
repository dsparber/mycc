using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.ExchangeRate.Database;
using MyCryptos.Core.ExchangeRate.Storage;

namespace MyCryptos.Core.ExchangeRate.Repositories
{
    public class LocalAvailableRatesRepository : AvailableRatesRepository
    {
        IEnumerable<Model.ExchangeRate> Elements;

        public LocalAvailableRatesRepository(string name) : base(AvailableRatesRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY, name)
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
                Elements = ExchangeRateStorage.Instance.LocalRepository.Elements;
                return true;
            });
        }

        public override Task<bool> LoadFromDatabase()
        {
            return FetchOnline();
        }

        public override ExchangeRateRepository ExchangeRateRepository
        {
            get
            {
                return ExchangeRateStorage.Instance.LocalRepository;
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