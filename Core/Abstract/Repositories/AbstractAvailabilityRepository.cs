namespace MyCryptos.Core.Repositories.Core
{
    public abstract class AbstractAvailabilityRepository<V> : AbstractRepository
    {
        protected AbstractAvailabilityRepository(int repositoryTypeId, string name) : base(repositoryTypeId, name) { }

        public abstract bool IsAvailable(V element);
    }
}