using System;

namespace TaskFlow.Domain.Entities;

public abstract class EntityBase
{
    public Guid Id { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; private set; }

    protected EntityBase()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public void UpdateAudit()
    {
        UpdatedAt = DateTime.UtcNow;
    }

     public override bool Equals(object? obj)
    {
        var otherEntity = obj as EntityBase;

        if (ReferenceEquals(otherEntity, null))
            return false;
        
        if (ReferenceEquals(this, otherEntity))
            return true;
        
        return Id == otherEntity.Id;
    }

    public static bool operator ==(EntityBase a, EntityBase b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            return true;

        if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(EntityBase a, EntityBase b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }

}
