using System;

namespace TaskFlow.Domain.Entities;

public class List : EntityBase
{
    public Guid UserId { get; private init; }
    public string Title { get; private set; }
    public string? Description { get; private set; }

    // EF Core
    private List(){}

    public List(
        Guid userId,
        string title,
        string? description = null
    ) : base()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        
        this.UserId = userId;
        this.Title = title;
        this.Description = description;
    }

    public void UpdateDetails(string title, string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(description));
    
        this.Title = title;
        this.Description = description;
        
        this.UpdateAudit();
    }
}
