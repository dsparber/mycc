namespace MyCC.Core.Assets.Models
{
    public class Asset
    {
        private AssetsSource _source;
        private decimal _amount;
        private string _currencyId;

        public AssetsSource Source => _source;
        public decimal Amount => _amount;
        public string CurrencyId => _currencyId;

        public Asset(AssetsSource source, decimal amount, string currencyId)
        {
            _source = source;
            _amount = amount;
            _currencyId = currencyId;
        }
    }
}