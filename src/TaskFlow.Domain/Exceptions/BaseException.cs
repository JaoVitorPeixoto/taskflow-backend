using System;

namespace TaskFlow.Domain.Exceptions;

public abstract class BaseException : Exception
{
    public string Code { get; private init; }
    public string Title { get; private init; }

    protected BaseException(
        string title,
        string code,
        string message
    ) : base(message)
    {
        this.Code = code;
        this.Title = title;
    }

}
