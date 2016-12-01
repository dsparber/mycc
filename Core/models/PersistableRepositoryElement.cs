namespace MyCryptos.models
{
	public interface PersistableRepositoryElement<IdType> : Persistable<IdType>
	{
		int RepositoryId { get; set; }
	}
}
