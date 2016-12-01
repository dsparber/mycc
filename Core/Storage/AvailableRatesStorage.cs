using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Database;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Models;
using MyCryptos.Core.Repositories.AvailableRates;

namespace MyCryptos.Core.Storage
{
    public class AvailableRatesStorage : AbstractStorage<AvailableRatesRepositoryDBM, AvailableRatesRepository>
    {
        public AvailableRatesStorage() : base(new AvailableRatesRepositoryDatabase()) { }

        protected override async Task OnFirstLaunch()
        {
            await Add(new BittrexAvailableRatesRepository(null));
            await Add(new LocalAvailableRatesRepository(null));
            await Add(new BtceAvailableRatesRepository(null));
            await Add(new CryptonatorAvailableRatesRepository(null));
        }

        static AvailableRatesStorage instance { get; set; }

        public static AvailableRatesStorage Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AvailableRatesStorage();
                }
                return instance;
            }
        }

        public ExchangeRate ExchangeRateWithCurrency(Currency currency)
        {
            foreach (var r in Repositories)
            {
                var e = r.ExchangeRateWithCurrency(currency);
                if (e != null)
                {
                    return e;
                }
            }
            return null;
        }

        public List<ExchangeRate> ExchangeRatesWithCurrency(Currency currency)
        {
            var all = new List<ExchangeRate>();
            foreach (var r in Repositories)
            {
                all.AddRange(r.ExchangeRatesWithCurrency(currency));

            }
            return all;
        }

        public bool IsAvailable(ExchangeRate exchangeRate)
        {
            foreach (var r in Repositories)
            {
                if (r.IsAvailable(exchangeRate))
                {
                    return true;
                }
            }
            return false;
        }
    }
}