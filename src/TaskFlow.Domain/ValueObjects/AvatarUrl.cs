using System;

namespace TaskFlow.Domain.ValueObjects;

public record AvatarUrl
{
    public string Url { get; private set; }

    public override string ToString() => Url;
    public static implicit operator string(AvatarUrl avatarUrl) => avatarUrl.ToString();

    public AvatarUrl(string url)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url, nameof(url));

        var normalizedUrl = new Uri(url.Trim());

        Url = normalizedUrl.AbsolutePath;
    }
}
