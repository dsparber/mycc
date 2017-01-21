using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.ExchangeRate.Database;
using MyCC.Core.ExchangeRate.Storage;

namespace MyCC.Core.ExchangeRate.Repositories
{
    public class LocalAvailableRatesRepository : AvailableRatesRepository
    {
        IEnumerable<Model.ExchangeRate> Elements;

        public LocalAvailableRatesRepository(int id) : base(id)
        {
            Elements = new List<Model.ExchangeRate>();
        }
        public override int RepositoryTypeId => AvailableRatesRepositoryDbm.DB_TYPE_LOCAL_REPOSITORY;

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