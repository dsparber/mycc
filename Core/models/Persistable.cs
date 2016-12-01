namespace MyCryptos.models
{
	public interface Persistable<IdType>
	{
		IdType Id { get; set; }
	}
}
