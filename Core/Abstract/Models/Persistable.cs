namespace MyCC.Core.Abstract.Models
{
    public interface IPersistable<TIdType>
    {
        TIdType Id { get; set; }
    }
}
