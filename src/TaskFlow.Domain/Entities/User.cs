using System;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.ValueObjects;

namespace TaskFlow.Domain.Entities;

public class User : EntityBase
{
    public string Name { get; private set;}
    public Email Email { get; private set; }
    public string Password { get; private set; }
    public AvatarUrl AvatarUrl { get; private set; }

    // Navegações
    private readonly List<List> _lists = new();
    private readonly List<Task> _tasks = new();

    public IReadOnlyCollection<List> Lists => _lists;
    public IReadOnlyCollection<Task> Tasks => _tasks;

    // EF Core
    private User() { }

    public User(
        string name,
        Email email,
        string password,
        AvatarUrl avartarUrl
    ) : base()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DataIsInvalidException("NAME_IS_EMPTY", "Name cannot be empty.");
        if (string.IsNullOrWhiteSpace(password))
            throw new DataIsInvalidException("PASSWORD_IS_EMPTY", "Password cannot be empty.");

        this.Name = name;
        this.Password = password;
        this.Email = email;
        this.AvatarUrl = avartarUrl;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DataIsInvalidException("NAME_IS_EMPTY", "Name cannot be empty.");

        this.Name = name;

        this.UpdateAudit();
    }

    public void UpdatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new DataIsInvalidException("PASSWORD_IS_EMPTY", "Password cannot be empty.");

        this.Password = password;

        this.UpdateAudit();
    }

    public void UpdateEmail(Email email)
    {
        this.Email = email;

        this.UpdateAudit();
    }

    public void UpdateAvatarUrl(AvatarUrl avatarUrl)
    {
        this.AvatarUrl = avatarUrl;

        this.UpdateAudit();
    }

}
