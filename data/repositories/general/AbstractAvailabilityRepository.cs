namespace data.repositories.general
{
	public abstract class AbstractAvailabilityRepository<V> : AbstractRepository<V>
	{
		protected AbstractAvailabilityRepository(int repositoryId, string name) : base(repositoryId, name) { }

		public abstract bool IsAvailable(V element);
	}
}