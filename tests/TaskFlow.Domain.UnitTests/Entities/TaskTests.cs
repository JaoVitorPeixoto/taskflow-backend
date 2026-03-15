using Bogus;
using FluentAssertions;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.ValueObjects;

namespace TaskFlow.Domain.UnitTests.Entities;

public class TaskTests
{
    private readonly Faker _faker = new Faker("pt_BR");

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "Task")]
    public void Constructor_GivenValidData_ShouldCreateTask()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var title = _faker.Lorem.Sentence();
        var description = _faker.Lorem.Paragraph();
        var priority = TaskPriority.P2;
        var scheduling = Scheduling.Factory.Once(DateOnly.FromDateTime(DateTime.Today), TimeOnly.FromDateTime(DateTime.Now));
        var notify = true;

        // Act
        var task = new Domain.Entities.Task(userId, title, description, null, priority, scheduling, notify);

        // Assert
        task.UserId.Should().Be(userId);
        task.Title.Should().Be(title);
        task.Description.Should().Be(description);
        task.Priority.Should().Be(priority);
        task.Scheduling.Should().Be(scheduling);
        task.Notify.Should().Be(notify);
        task.IsCompleted.Should().BeFalse();
        task.CompletedAt.Should().BeNull();
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "Task")]
    public void Constructor_GivenEmptyTitle_ShouldThrowDataIsInvalidException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        Action act = () => new Domain.Entities.Task(userId, "", null);

        // Assert
        act.Should().Throw<DataIsInvalidException>()
            .WithMessage("Title cannot be empty.")
            .And.Code.Should().Be("TITLE_IS_EMPTY");
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "Task")]
    public void Constructor_GivenNotifyWithoutTime_ShouldThrowDataIsInvalidException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var title = _faker.Lorem.Sentence();
        var description = _faker.Lorem.Paragraph();
        var priority = TaskPriority.P2;
        var scheduling = Scheduling.Factory.Once(DateOnly.FromDateTime(DateTime.Today)); // Sem horário definido
        var notify = true;

        // Act
        Action act = () => new Domain.Entities.Task(userId, title, description, null, priority, scheduling, notify);

        // Assert
        act.Should().Throw<DataIsInvalidException>()
            .WithMessage("In order to issue a notification, the scheduling and time cannot be null.")
            .And.Code.Should().Be("NOTIFY_WITH_SCHEDULING_TIME_IS_NULL");
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "Task")]
    public void UpdateDetails_GivenValidData_ShouldUpdateTitleAndDescription()
    {
        // Arrange
        var task = CreateValidTask();
        var newTitle = _faker.Lorem.Sentence();
        var newDescription = _faker.Lorem.Paragraph();

        // Act
        task.UpdateDetails(newTitle, newDescription);

        // Assert
        task.Title.Should().Be(newTitle);
        task.Description.Should().Be(newDescription);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "Task")]
    public void UpdateDetails_GivenEmptyTitle_ShouldThrowDataIsInvalidException()
    {
        // Arrange
        var task = CreateValidTask();

        // Act
        Action act = () => task.UpdateDetails("", null);

        // Assert
        act.Should().Throw<DataIsInvalidException>()
            .WithMessage("Title cannot be empty.")
            .And.Code.Should().Be("TITLE_IS_EMPTY");
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "Task")]
    public void Complete_GivenNonRecurringTask_ShouldMarkAsCompleted()
    {
        // Arrange
        var task = CreateValidTask();

        // Act
        var newTask = task.Complete();

        // Assert
        task.IsCompleted.Should().BeTrue();
        task.CompletedAt.Should().NotBeNull();
        newTask.Should().BeNull();
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "Task")]
    public void Complete_GivenRecurringTask_ShouldReturnNewTask()
    {
        // Arrange
        var scheduling = Scheduling.Factory.Recurring(DateOnly.FromDateTime(DateTime.Today), null, RecurrenceSchedule.Daily);
        var task = new Domain.Entities.Task(Guid.NewGuid(), _faker.Lorem.Sentence(), null, null, TaskPriority.P3, scheduling, false);

        // Act
        var newTask = task.Complete();

        // Assert
        task.IsCompleted.Should().BeTrue();
        task.CompletedAt.Should().NotBeNull();
        newTask.Should().NotBeNull();
        newTask.Scheduling.Should().NotBeNull();
        newTask.Scheduling!.Date.Should().Be(scheduling.Date.AddDays(1));
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "Task")]
    public void Compare_GivenTwoEqualTasks_ShouldBeEqual()
    {
        // Arrange
        var commonTask = CreateValidTask();
        var task1 = commonTask;
        var task2 = commonTask;

        // Act & Assert
        task1.Should().Be(task2);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "Task")]
    public void Compare_GivenTwoDifferentTasks_ShouldNotBeEqual()
    {
        // Arrange
        var task1 = new Domain.Entities.Task(Guid.NewGuid(), "Task 1", null);
        var task2 = new Domain.Entities.Task(Guid.NewGuid(), "Task 2", null);

        // Act & Assert
        task1.Should().NotBe(task2);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "Task")]
    public void RemoveNotify_GivenTaskWithNotification_ShouldDisableNotification()
    {
        // Arrange
        var task = CreateValidTask();
        task.ShouldNotify();

        // Act
        task.RemoveNotify();

        // Assert
        task.Notify.Should().BeFalse();
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "Task")]
    public void UpdatePriority_GivenNewPriority_ShouldUpdatePriority()
    {
        // Arrange
        var task = CreateValidTask();
        var newPriority = TaskPriority.P1;

        // Act
        task.UpdatePriority(newPriority);

        // Assert
        task.Priority.Should().Be(newPriority);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "Task")]
    public void MoveToList_GivenValidListId_ShouldUpdateListId()
    {
        // Arrange
        var task = CreateValidTask();
        var newListId = Guid.NewGuid();

        // Act
        task.MoveToList(newListId);

        // Assert
        task.ListId.Should().Be(newListId);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "Task")]
    public void RemoveList_GivenTaskWithList_ShouldClearListId()
    {
        // Arrange
        var task = CreateValidTask();
        var listId = Guid.NewGuid();
        task.MoveToList(listId);

        // Act
        task.RemoveList();

        // Assert
        task.ListId.Should().BeNull();
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "Task")]
    public void RemoveScheduling_GivenTaskWithScheduling_ShouldClearSchedulingAndNotify()
    {
        // Arrange
        var task = CreateValidTask();

        // Act
        task.RemoveScheduling();

        // Assert
        task.Scheduling.Should().BeNull();
        task.Notify.Should().BeFalse();
    }

    private Domain.Entities.Task CreateValidTask()
    {
        return new Domain.Entities.Task(
            Guid.NewGuid(),
            _faker.Lorem.Sentence(),
            _faker.Lorem.Paragraph(),
            null,
            TaskPriority.P3,
            Scheduling.Factory.Once(DateOnly.FromDateTime(DateTime.Today)),
            false
        );
    }
}
