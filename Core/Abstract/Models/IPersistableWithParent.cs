namespace MyCC.Core.Abstract.Models
{
    public interface IPersistableWithParent<TIdType> : IPersistable<TIdType>
    {
        int ParentId { get; set; }
    }
}
