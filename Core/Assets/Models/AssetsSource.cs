using System.Collections.Generic;
using MyCC.Core.Assets.Sources;

namespace MyCC.Core.Assets.Models
{
    public abstract class AssetsSource
    {
        private int? _id;
        private string _name;
        internal abstract AssetsSourceType Type { get; }

        private decimal? _amount;
        private string _currencyId;
        private string _address;
        private string _publicKey;
        private string _privateKey;

        private IEnumerable<Asset> _assets;

        public int? Id
        {
            get => _id;
            internal set => _id = value;
        }

        public string Name {
            get => _name;
            set => _name = value;
        }


        public decimal? Amount
        {
            get => _amount;
            set => _amount = value;
        }

        public string CurrencyId
        {
            get => _currencyId;
            set => _currencyId = value;
        }

        public string Address
        {
            get => _address;
            set => _address = value;
        }

        public string PublicKey
        {
            get => _publicKey;
            set => _publicKey = value;
        }

        public string PrivateKey
        {
            get => _privateKey;
            set => _privateKey = value;
        }

        public IEnumerable<Asset> Assets
        {
            get => _assets ?? new List<Asset>();
            internal set => _assets = value;
        }
    }
}