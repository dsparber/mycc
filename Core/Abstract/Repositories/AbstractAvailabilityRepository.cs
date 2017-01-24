namespace MyCC.Core.Abstract.Repositories
{
    public abstract class AbstractAvailabilityRepository<TV> : AbstractRepository
    {
        protected AbstractAvailabilityRepository(int id) : base(id) { }

        public abstract bool IsAvailable(TV element);
    }
}