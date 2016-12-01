namespace MyCryptos.Core.Repositories.AvailableRates
{
    public abstract class OnlineAvailableRatesRepository : AvailableRatesRepository
    {
        protected OnlineAvailableRatesRepository(int repositoryId, string name) : base(repositoryId, name) { }
    }
}

