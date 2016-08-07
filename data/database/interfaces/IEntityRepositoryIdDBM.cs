namespace data.database.interfaces
{
	public interface IEntityRepositoryIdDBM<T> : IEntityDBM<T>
	{
		int RepositoryId { get; set; }
	}
}

