using System;
using MyCC.Core.Assets.Models;
using MyCC.Core.Assets.Sources;

namespace MyCC.Core.Assets.Utils
{
    public static class AssetsFactory
    {
        public static AssetsSource CreateWithAmount(string name, string currencyId, decimal amount) =>
            CreateWithAmount(null, name, currencyId, amount);

        public static AssetsSource CreateWithAddress(string name, string currencyId, string address) =>
            CreateWithAddress(null, name, currencyId, address);

        public static AssetsSource CreateForBittrex(string name, string publicKey, string privateKey) =>
            CreateForBittrex(null, name, publicKey, privateKey);

        public static AssetsSource CreateForPoloniex(string name, string publicKey, string privateKey) =>
            CreateForPoloniex(null, name, publicKey, privateKey);


        internal static AssetsSource CreateWithAmount(int? id, string name, string currencyId, decimal amount)
            => new AssetsAmountSource(id, name, currencyId, amount);

        internal static AssetsSource CreateWithAddress(int? id, string name, string currencyId, string address)
        {
            throw new NotImplementedException();
        }

        internal static AssetsSource CreateForBittrex(int? id, string name, string publicKey, string privateKey)
        {
            throw new NotImplementedException();
        }

        internal static AssetsSource CreateForPoloniex(int? id, string name, string publicKey, string privateKey)
        {
            throw new NotImplementedException();
        }
    }
}