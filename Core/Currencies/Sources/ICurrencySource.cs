using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Currencies.Models;

namespace MyCC.Core.Currencies.Sources
{
    public interface ICurrencySource
    {
        /// <summary>
        /// Descriptive name of the source
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Fetches all available currencies online
        /// </summary>
        /// <returns>Fetched currencies</returns>
        Task<IEnumerable<Currency>> GetCurrencies();
    }
}

