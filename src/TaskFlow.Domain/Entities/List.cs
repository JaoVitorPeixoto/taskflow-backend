using System;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Domain.Entities;

public class List : EntityBase
{
    public Guid UserId { get; private init; }
    public string Title { get; private set; }
    public string? Description { get; private set; }


    // Navegações
    public User User { get; private set; }
    public IReadOnlyCollection<Task> Tasks { get; private set; } = [];

    // EF Core
    private List(){}

    public List(
        Guid userId,
        string title,
        string? description = null
    ) : base()
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DataIsInvalidException("TITLE_IS_EMPTY", "Title cannot be empty.");

        this.UserId = userId;
        this.Title = title;
        this.Description = description;
    }

    public void UpdateDetails(string title, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DataIsInvalidException("TITLE_IS_EMPTY", "Title cannot be empty.");

        this.Title = title;
        this.Description = description;

        this.UpdateAudit();
    }
}
