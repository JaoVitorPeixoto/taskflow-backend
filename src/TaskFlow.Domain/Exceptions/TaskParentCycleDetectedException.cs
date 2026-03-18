using System;

namespace TaskFlow.Domain.Exceptions;

public class TaskParentCycleDetectedException : DomainException
{
    public TaskParentCycleDetectedException(Guid taskId, Guid newParentTaskId)
        : base(
            "TASK_PARENT_CYCLE_DETECTED",
            $"Cannot set parent task. This operation would create a cycle. TaskId: {taskId}, ParentTaskId: {newParentTaskId}.")
    {
    }
}
