namespace data.database.interfaces
{
	public interface IRepositoryIdDBM<T> : IDBM<T>
	{
		int RepositoryId { get; set; }
	}
}

