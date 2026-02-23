using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Domain.Entities.Base
{
    public abstract class BaseEntity : IAuditableEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
