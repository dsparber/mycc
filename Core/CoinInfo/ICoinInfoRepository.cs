﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCC.Core.CoinInfo
{
    public interface ICoinInfoRepository
    {
        /// <summary>
        /// Gets the supported coins of this repository.
        /// </summary>
        /// <value>The supported coins.</value>
        List<Currencies.Model.Currency> SupportedCoins { get; }

        /// <summary>
        /// Loads the info of the specified coin online. 
        /// </summary>
        /// <returns>The info for this coin</returns>
        /// <param name="currency">Currency of the desired coin</param>
        Task<CoinInfoData> GetInfo(Currencies.Model.Currency currency);

        /// <summary>
        /// Gets the name of the repository
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }
    }
}
