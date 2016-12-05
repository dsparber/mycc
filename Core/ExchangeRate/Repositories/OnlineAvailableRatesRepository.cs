namespace MyCryptos.Core.ExchangeRate.Repositories
{
	public abstract class OnlineAvailableRatesRepository : AvailableRatesRepository
	{
		protected OnlineAvailableRatesRepository(int id) : base(id) { }
	}
}

