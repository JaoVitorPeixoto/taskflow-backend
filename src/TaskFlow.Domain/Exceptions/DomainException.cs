using System;

namespace TaskFlow.Domain.Exceptions;

public abstract class DomainException : BaseException
{

    protected DomainException(
        string code, 
        string message
    ) : base ("Domain Rule Violation", code, message)
    {
        
    }
}
