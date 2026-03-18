using System;

namespace TaskFlow.Domain.Exceptions;

public class TaskIsOwnParentException : DomainException
{
    public TaskIsOwnParentException()
        : base(
            "TASK_IS_OWN_PARENT",
            "A task cannot be assigned as a child of itself.")
    {
    }
}
