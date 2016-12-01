namespace MyCryptos.Core.Models
{
    public interface PersistableRepositoryElement<IdType> : Persistable<IdType>
    {
        int RepositoryId { get; set; }
    }
}
