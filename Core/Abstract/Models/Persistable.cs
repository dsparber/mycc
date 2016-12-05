namespace MyCryptos.Core.Models
{
    public interface Persistable<IdType>
    {
        IdType Id { get; set; }
    }
}
