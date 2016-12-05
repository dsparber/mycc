namespace MyCryptos.Core.Models
{
	public interface IPersistableWithParent<IdType> : Persistable<IdType>
	{
		int ParentId { get; set; }
	}
}
