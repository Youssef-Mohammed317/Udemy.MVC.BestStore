namespace BestStore.Domain.Entities.Base
{
    public interface IAuditableEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
