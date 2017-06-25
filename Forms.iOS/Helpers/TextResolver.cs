using MyCC.Forms.iOS.Helpers;
using MyCC.Forms.Resources;
using MyCC.Ui.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(TextResolver))]
namespace MyCC.Forms.iOS.Helpers
{
    public class TextResolver : ITextResolver
    {
        public string ManuallyAdded => I18N.ManuallyAdded;
        public string AddedWith => I18N.AddedWith;
        public string AddressAdded => I18N.AddressAdded;
        public string Amount => I18N.Amount;
        public string Currency => I18N.Currency;
        public string Name => I18N.Name;
        public string OneAccount => I18N.OneAccount;
        public string Accounts => I18N.Accounts;
        public string Currencies => I18N.Currencies;
        public string OneCurrency => I18N.OneCurrency;
        public string Further => I18N.Further;
        public string NoDataToDisplay => I18N.NoDataToDisplay;
        public string AsCurrency => I18N.AsCurrency;
        public string CoinProofOfWork => I18N.CoinProofOfWork;
        public string CoinProofOfStake => I18N.CoinProofOfStake;
        public string GHps => I18N.GHps;
        public string UnitSecond => I18N.UnitSecond;
        public string LoadingCurrenciesFrom => I18N.LoadingCurrenciesFrom;
        public string LoadingRates => I18N.LoadingRates;
        public string OnlyThreeCurrenciesCanBeStared => I18N.OnlyThreeCurrenciesCanBeStared;
        public string BitcoinCanNotBeRemoved => I18N.BitcoinCanNotBeRemoved;
        public string IsEqualTo => I18N.IsEqualTo;
        public string AreEqualTo => I18N.AreEqualTo;
        public string FetchingNoSuccessText => I18N.FetchingNoSuccessText;
        public string VerifyInput => I18N.VerifyInput;
    }
}