using System;
using TaskFlow.Domain.Exceptions;
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
        if (string.IsNullOrWhiteSpace(name))
            throw new DataIsInvalidException("NAME_IS_EMPTY", "Name cannot be empty.");
        if (string.IsNullOrWhiteSpace(password))
            throw new DataIsInvalidException("PASSWORD_IS_EMPTY", "Password cannot be empty.");

        this.Name = name;
        this.Password = password;
        this.Email = email;
        this.AvartarUrl = avartarUrl;
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
        this.AvartarUrl = avatarUrl;

        this.UpdateAudit();
    }

}
