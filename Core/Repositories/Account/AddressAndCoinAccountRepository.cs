using Newtonsoft.Json;

namespace MyCryptos.Core.Repositories.Account
{
    public abstract class AddressAndCoinAccountRepository : AddressAccountRepository
    {
        private readonly Models.Currency coin;
        protected sealed override Models.Currency Currency => coin;

        public override string Data { get { return JsonConvert.SerializeObject(new KeyData(coin, Address)); } }

        protected AddressAndCoinAccountRepository(int type, string name, string data) : this(type, name, null, null)
        {
            var keyData = JsonConvert.DeserializeObject<KeyData>(data);

            coin = keyData.coin;
            Address = keyData.address;
        }

        protected AddressAndCoinAccountRepository(int type, string name, Models.Currency coin, string address) : base(type, name, address)
        {
            this.coin = coin;
        }

        class KeyData
        {

            public string address;
            public Models.Currency coin;

            public KeyData(Models.Currency coin, string address)
            {
                this.coin = coin;
                this.address = address;
            }
        }

        public sealed override string Description => $"{DescriptionName} ({coin.Code})";
        public abstract string DescriptionName { get; }
    }
}
