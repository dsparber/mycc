namespace MyCC.Ui.Get
{
    public interface IGetUtils
    {
        IAssetsOverviewData Assets { get; }
        IRatesOverviewData Rates { get; }
        ICoinInfoViewData CoinInfoViewData { get; }
        IAccountDetailViewData AccountDetail { get; }
        IAccountsGroupViewData AccountsGroup { get; }
    }
}