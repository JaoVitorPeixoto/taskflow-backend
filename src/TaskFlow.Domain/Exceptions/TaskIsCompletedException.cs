using System;

namespace TaskFlow.Domain.Exceptions;

public class TaskIsCompletedException : DomainException
{
    public TaskIsCompletedException()
    : base("TASK_ALREADY_COMPLETED", "The action cannot be performed; the task has already been completed.")
    {
        
    }
}
