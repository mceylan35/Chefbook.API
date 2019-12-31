using System;

namespace Chefbook.Model
{
    public interface CoreEntity
    {
        Guid Id { get; set; }
        DateTime DataCreated { get; set; }
        bool IsDeleted { get; set; }
        
    }
}