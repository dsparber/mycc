namespace MyCC.Ui.Helpers
{
    public interface ITextResolver
    {
        string ManuallyAdded { get; }
        string AddedWith { get; }
        string AddressAdded { get; }
        string Amount { get; }
        string Currency { get; }
        string Name { get; }
        string OneAccount { get; }
        string Accounts { get; }
        string Currencies { get; }
        string OneCurrency { get; }
        string Further { get; }
        string NoDataToDisplay { get; }
        string AsCurrency { get; }
        string CoinProofOfWork { get; }
        string CoinProofOfStake { get; }
        string GHps { get; }
        string UnitSecond { get; }
        string LoadingCurrenciesFrom { get; }
        string LoadingRates { get; }
        string OnlyThreeCurrenciesCanBeStared { get; }
        string BitcoinCanNotBeRemoved { get; }
        string IsEqualTo { get; }
        string AreEqualTo { get; }
        string FetchingNoSuccessText { get; }
        string VerifyInput { get; }
    }
}