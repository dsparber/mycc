using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Models;
using MyCryptos.Core.Repositories.ExchangeRates;
using MyCryptos.Core.Storage;

namespace MyCryptos.Core.Repositories.AvailableRates
{
    public class LocalAvailableRatesRepository : AvailableRatesRepository
    {
        IEnumerable<ExchangeRate> Elements;

        public LocalAvailableRatesRepository(string name) : base(AvailableRatesRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY, name)
        {
            Elements = new List<ExchangeRate>();
        }

        public override bool IsAvailable(ExchangeRate element)
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

        public override ExchangeRate ExchangeRateWithCurrency(Models.Currency currency)
        {
            return Elements.ToList().Find(e => e.Contains(currency));
        }

        public override List<ExchangeRate> ExchangeRatesWithCurrency(Models.Currency currency)
        {
            return Elements.Where(e => e.Contains(currency)).ToList();
        }
    }
}