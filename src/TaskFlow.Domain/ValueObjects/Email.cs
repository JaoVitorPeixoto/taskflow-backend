using System;
using System.Net.Mail;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Domain.ValueObjects;

public record Email
{
    public string Address { get; private set; }

    public override string ToString() => Address;
    public static implicit operator string(Email email) => email.Address.ToString();

    public Email(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new DataIsInvalidException("EMAIL_IS_EMPTY", "Email address cannot be empty.");

        var normalized = address.Trim().ToLowerInvariant(); // normaliza primeiro

        if (!IsValidEmail(normalized))
            throw new DataIsInvalidException("EMAIL_INCORRET_FORMAT", "Invalid email address format.");

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
