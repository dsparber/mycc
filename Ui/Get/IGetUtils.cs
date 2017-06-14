namespace MyCC.Ui.Get
{
    public interface IGetUtils
    {
        IAssetsOverviewData Assets { get; }
        IRatesOverviewData Rates { get; }
        ICoinInfoViewData CoinInfo { get; }
        IAccountDetailViewData AccountDetail { get; }
        IAccountsGroupViewData AccountsGroup { get; }
    }
}