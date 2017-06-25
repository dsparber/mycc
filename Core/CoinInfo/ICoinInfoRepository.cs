using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCC.Core.CoinInfo
{
    public interface ICoinInfoRepository
    {
        /// <summary>
        /// Gets the supported coins of this repository.
        /// </summary>
        /// <value>The supported coins.</value>
        List<string> SupportedCoins { get; }

        /// <summary>
        /// Loads the info of the specified coin online. 
        /// </summary>
        /// <returns>The info for this coin</returns>
        /// <param name="currencyId">Currency of the desired coin</param>
        Task<CoinInfoData> GetInfo(string currencyId);

        /// <summary>
        /// Gets the name of the repository
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        string WebUrl(string currencyId);
    }
}
