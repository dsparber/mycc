using System.Collections.Generic;
using MyCC.Core.Assets.Models;

namespace MyCC.Core.Assets.Sources
{
    internal class AssetsAmountSource : AssetsSource
    {
        internal override AssetsSourceType Type => AssetsSourceType.WithAmount;

        public AssetsAmountSource(int? id, string name, string currencyId, decimal amount)
        {
            Id = id;
            Name = name;
            CurrencyId = currencyId;
            Amount = amount;
            var assets = new List<Asset> {new Asset(this, amount, currencyId)};
            Assets = assets;
        }
    }
}