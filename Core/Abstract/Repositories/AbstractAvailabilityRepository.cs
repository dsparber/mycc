namespace MyCryptos.Core.Abstract.Repositories
{
	public abstract class AbstractAvailabilityRepository<V> : AbstractRepository
	{
		protected AbstractAvailabilityRepository(int id) : base(id) { }

		public abstract bool IsAvailable(V element);
	}
}