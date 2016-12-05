namespace MyCryptos.Core.ExchangeRate.Repositories
{
    public abstract class OnlineAvailableRatesRepository : AvailableRatesRepository
    {
        protected OnlineAvailableRatesRepository(int repositoryId, string name) : base(repositoryId, name) { }
    }
}

