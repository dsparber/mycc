namespace MyCryptos.models
{
	public interface PersistableRepositoryElement : Persistable
	{
		int? RepositoryId { get; }
	}
}
