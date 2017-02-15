using Newtonsoft.Json;

namespace MyCC.Core.Account.Repositories.Base
{
    public abstract class AddressAndCoinAccountRepository : AddressAccountRepository
    {
        private readonly Currency.Model.Currency _coin;
        public sealed override Currency.Model.Currency Currency => _coin;

        public override string Data => JsonConvert.SerializeObject(new KeyData(_coin, Address));

        protected AddressAndCoinAccountRepository(int id, string name, string data) : this(id, name, null, null)
        {
            var keyData = JsonConvert.DeserializeObject<KeyData>(data);

            _coin = keyData.Coin;
            if (_coin != null) _coin.IsCryptoCurrency = true;
            Address = keyData.address;
        }

        protected AddressAndCoinAccountRepository(int id, string name, Currency.Model.Currency coin, string address) : base(id, name, address)
        {
            if (coin == null) return;

            _coin = coin;
            _coin.IsCryptoCurrency = true;
        }

        private class KeyData
        {

            public string address;
            public Currency.Model.Currency Coin;

            public KeyData(Currency.Model.Currency coin, string address)
            {
                Coin = coin;
                this.address = address;
            }
        }

        public sealed override string Description => $"{DescriptionName} ({_coin.Code})";
        public abstract string DescriptionName { get; }
    }
}
