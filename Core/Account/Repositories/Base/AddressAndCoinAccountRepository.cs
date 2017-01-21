using Newtonsoft.Json;

namespace MyCC.Core.Account.Repositories.Base
{
    public abstract class AddressAndCoinAccountRepository : AddressAccountRepository
    {
        private readonly Currency.Model.Currency coin;
        protected sealed override Currency.Model.Currency Currency => coin;

        public override string Data { get { return JsonConvert.SerializeObject(new KeyData(coin, Address)); } }

        protected AddressAndCoinAccountRepository(int id, string name, string data) : this(id, name, null, null)
        {
            var keyData = JsonConvert.DeserializeObject<KeyData>(data);

            coin = keyData.coin;
            Address = keyData.address;
        }

        protected AddressAndCoinAccountRepository(int id, string name, Currency.Model.Currency coin, string address) : base(id, name, address)
        {
            this.coin = coin;
        }

        class KeyData
        {

            public string address;
            public Currency.Model.Currency coin;

            public KeyData(Currency.Model.Currency coin, string address)
            {
                this.coin = coin;
                this.address = address;
            }
        }

        public sealed override string Description => $"{DescriptionName} ({coin.Code})";
        public abstract string DescriptionName { get; }
    }
}
