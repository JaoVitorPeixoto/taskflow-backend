using System;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Domain.ValueObjects;

public record AvatarUrl
{
    public string Url { get; private set; }

    public override string ToString() => Url;
    public static implicit operator string(AvatarUrl avatarUrl) => avatarUrl.ToString();

      // EF Core
    private AvatarUrl() {}

    public AvatarUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new DataIsInvalidException("URL_IS_EMPTY", "Avatar URL cannot be empty.");

        try
        {
            var normalizedUrl = new Uri(url.Trim());

            Url = normalizedUrl.AbsoluteUri;
        }
        catch
        {
            throw new DataIsInvalidException("URL_INCORRECT_FORMAT", "Avatar URL is in incorrect format.");
        }   
        
    }
}
