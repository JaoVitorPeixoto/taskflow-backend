using System;
using TaskFlow.Domain.ValueObjects;

namespace TaskFlow.Domain.Entities;

public class User : EntityBase
{
    public string Name { get; private set;}
    public Email Email { get; private set; }
    public string Password { get; private set; }
    public AvatarUrl AvartarUrl { get; private set; }

    // EF Core
    private User() { }

    public User(
        string name,
        Email email,
        string password,
        AvatarUrl avartarUrl
    ) : base()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));

        this.Name = name;
        this.Password = password;
        this.Email = email;
        this.AvartarUrl = avartarUrl;
    }

    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        this.Name = name;

        this.UpdateAudit();s
    }

    public void UpdatePassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));

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
        this.AvartarUrl = avatarUrl;

        this.UpdateAudit();
    }

}
