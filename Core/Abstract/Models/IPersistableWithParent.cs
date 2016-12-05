namespace MyCryptos.Core.Abstract.Models
{
    public interface IPersistableWithParent<IdType> : Persistable<IdType>
    {
        int ParentId { get; set; }
    }
}
