using System;

namespace TaskFlow.Domain.Exceptions;

public class DataIsInvalidException : DomainException
{

    public DataIsInvalidException(
        string code,
        string message
    ) : base(code, message)
    {
        
    }
}
