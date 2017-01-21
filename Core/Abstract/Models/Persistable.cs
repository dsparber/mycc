namespace MyCC.Core.Abstract.Models
{
    public interface Persistable<IdType>
    {
        IdType Id { get; set; }
    }
}
