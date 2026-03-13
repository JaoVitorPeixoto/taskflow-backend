using System;
using System.Net.Mail;

namespace TaskFlow.Domain.ValueObjects;

public record Email
{
    public string Address { get; private set; }

    public override string ToString() => Address;
    public static implicit operator string(Email email) => email.Address.ToString();

    public Email(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Email address cannot be empty.", nameof(address));

        var normalized = address.Trim().ToLowerInvariant(); // normaliza primeiro

        if (!IsValidEmail(normalized))
            throw new ArgumentException("Invalid email address format.", nameof(address));

        Address = normalized;
    }

    private static bool IsValidEmail(string address)
    {
        try
        {
            var addr = new MailAddress(address);
            return addr.Address == address;
        }
        catch
        {
            return false;
        }
    }
}
