using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Storage;
using MyCryptos.Core.ExchangeRate.Database;
using MyCryptos.Core.ExchangeRate.Repositories;

// TODO make private? 
namespace MyCryptos.Core.ExchangeRate.Storage
{
    public class AvailableRatesStorage : AbstractStorage<AvailableRatesRepositoryDBM, AvailableRatesRepository>
    {
        public AvailableRatesStorage() : base(new AvailableRatesRepositoryDatabase()) { }

        protected override async Task OnFirstLaunch()
        {
            await Add(new BittrexAvailableRatesRepository(null));
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

        public Model.ExchangeRate ExchangeRateWithCurrency(Currency.Model.Currency currency)
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

        public List<Model.ExchangeRate> ExchangeRatesWithCurrency(Currency.Model.Currency currency)
        {
            var all = new List<Model.ExchangeRate>();
            foreach (var r in Repositories)
            {
                all.AddRange(r.ExchangeRatesWithCurrency(currency));

            }
            return all;
        }

        public bool IsAvailable(Model.ExchangeRate exchangeRate)
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