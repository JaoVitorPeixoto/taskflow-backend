using System;
using Bogus;
using FluentAssertions;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.ValueObjects;
using TaskFlow.Infrastructure.Persistence.Repositories;
using Task = System.Threading.Tasks.Task;
using User = TaskFlow.Domain.Entities.User;
using TaskEntity = TaskFlow.Domain.Entities.Task;
using ListEntity = TaskFlow.Domain.Entities.List;

namespace TaskFlow.Infrastructure.UnitTests.Persistence.Repositories;

public class TaskRepositoryTests : IAsyncLifetime
{
	private readonly Faker _faker = new("pt_BR");

	private SqliteTestDb _db = null!;
	private UnitOfWork _uow = null!;

	private User _user1 = null!;
	private User _user2 = null!;

	private ListEntity _list1 = null!;
	private ListEntity _list2 = null!;

	private TaskEntity _task1 = null!;
	private TaskEntity _task2 = null!;
	private TaskEntity _task3 = null!;
	private TaskEntity _subTask1 = null!;
	private TaskEntity _todayTask1 = null!;
	private TaskEntity _todayTask2Completed = null!;

	public async Task InitializeAsync()
	{
		_db = new SqliteTestDb();
		_uow = new UnitOfWork(_db.Context);

		_user1 = new User(
			_faker.Name.FullName(),
			new Email(_faker.Internet.Email()),
			_faker.Internet.Password(),
			new AvatarUrl(_faker.Internet.Avatar())
		);

		_user2 = new User(
			_faker.Name.FullName(),
			new Email(_faker.Internet.Email()),
			_faker.Internet.Password(),
			new AvatarUrl(_faker.Internet.Avatar())
		);

		await _uow.UserRepository.AddAsync(_user1);
		await _uow.UserRepository.AddAsync(_user2);
		await _uow.CommitAsync();

		_list1 = new ListEntity(
			_user1.Id,
			_faker.Lorem.Sentence(4),
			_faker.Lorem.Text()
		);

		_list2 = new ListEntity(
			_user2.Id,
			_faker.Lorem.Sentence(4),
			_faker.Lorem.Text()
		);

		await _uow.ListRepository.AddAsync(_list1);
		await _uow.ListRepository.AddAsync(_list2);
		await _uow.CommitAsync();

		var today = DateOnly.FromDateTime(DateTime.Today);

		_task1 = new TaskEntity(
			_user1.Id,
			_faker.Lorem.Sentence(3),
			_faker.Lorem.Text(),
			_list1.Id,
			TaskPriority.P2
		);

		_task2 = new TaskEntity(
			_user1.Id,
			_faker.Lorem.Sentence(3),
			_faker.Lorem.Text(),
			_list1.Id,
			TaskPriority.P3
		);
		_task2.Complete();

		_task3 = new TaskEntity(
			_user2.Id,
			_faker.Lorem.Sentence(3),
			_faker.Lorem.Text(),
			_list2.Id,
			TaskPriority.P1
		);

		_subTask1 = new TaskEntity(
			_user1.Id,
			_faker.Lorem.Sentence(3),
			_faker.Lorem.Text(),
			_list1.Id,
			TaskPriority.P4
		);
		_subTask1.SetParentTask(_task1.Id);

		_todayTask1 = new TaskEntity(
			_user1.Id,
			_faker.Lorem.Sentence(3),
			_faker.Lorem.Text(),
			_list1.Id,
			TaskPriority.P4,
			Scheduling.Factory.Once(today, new TimeOnly(10, 30)),
			notify: true
		);

		_todayTask2Completed = new TaskEntity(
			_user1.Id,
			_faker.Lorem.Sentence(3),
			_faker.Lorem.Text(),
			_list1.Id,
			TaskPriority.P4,
			Scheduling.Factory.Once(today, new TimeOnly(16, 0)),
			notify: true
		);
		_todayTask2Completed.Complete();

		await _uow.TaskRepository.AddAsync(_task1);
		await _uow.TaskRepository.AddAsync(_task2);
		await _uow.TaskRepository.AddAsync(_task3);
		await _uow.TaskRepository.AddAsync(_subTask1);
		await _uow.TaskRepository.AddAsync(_todayTask1);
		await _uow.TaskRepository.AddAsync(_todayTask2Completed);
		await _uow.CommitAsync();
	}

	public async Task DisposeAsync()
	{
		await _uow.DisposeAsync();
		await _db.DisposeAsync();
	}

	[Fact]
	[Trait("Modulo", "Infrastructure")]
	[Trait("Repository", "TaskRepository")]
	public async Task Get_GivenValidTaskId_ShouldReturnTask()
	{
		// Arrange
		var expectedTask = _task1;

		// Act
		var actualTask = await _uow.TaskRepository.GetByIdAsync(expectedTask.Id);

		// Assert
		actualTask.Should().NotBeNull();
		actualTask.Should().Be(expectedTask);
	}

	[Fact]
	[Trait("Modulo", "Infrastructure")]
	[Trait("Repository", "TaskRepository")]
	public async Task Get_GivenNonExistentTaskId_ShouldReturnNull()
	{
		// Arrange
		var nonExistentTaskId = Guid.NewGuid();

		// Act
		var actualTask = await _uow.TaskRepository.GetByIdAsync(nonExistentTaskId);

		// Assert
		actualTask.Should().BeNull();
	}

	[Fact]
	[Trait("Modulo", "Infrastructure")]
	[Trait("Repository", "TaskRepository")]
	public async Task GetAll_GivenExistingTasksInDb_ShouldReturnAllTasks()
	{
		// Arrange
		var expectedTasks = new[]
		{
			_task1,
			_task2,
			_task3,
			_subTask1,
			_todayTask1,
			_todayTask2Completed
		};

		// Act
		var actualTasks = await _uow.TaskRepository.GetAllAsync();

		// Assert
		actualTasks.Should().BeEquivalentTo(expectedTasks);
	}

	[Fact]
	[Trait("Modulo", "Infrastructure")]
	[Trait("Repository", "TaskRepository")]
	public async Task GetAll_GivenFilter_ShouldReturnOnlyMatchedTasks()
	{
		// Arrange
		var expectedTaskId = _task1.Id;

		// Act
		var actualTasks = await _uow.TaskRepository.GetAllAsync(t => t.Id == expectedTaskId);

		// Assert
		actualTasks.Should().HaveCount(1);
		actualTasks.Should().ContainSingle(t => t.Id == expectedTaskId);
	}

	[Fact]
	[Trait("Modulo", "Infrastructure")]
	[Trait("Repository", "TaskRepository")]
	public async Task GetAllByUserId_GivenOnlyIncompleteFilter_ShouldReturnOnlyIncompleteUserTasks()
	{
		// Arrange
		var expectedUserId = _user1.Id;

		// Act
		var actualTasks = await _uow.TaskRepository.GetAllTasksByUserIdAsync(expectedUserId, TaskFilter.OnlyIncomplete);

		// Assert
		actualTasks.Should().OnlyContain(t => t.UserId == expectedUserId && !t.IsCompleted);
		actualTasks.Should().Contain(t => t.Id == _task1.Id);
		actualTasks.Should().Contain(t => t.Id == _subTask1.Id);
		actualTasks.Should().Contain(t => t.Id == _todayTask1.Id);
	}

	[Fact]
	[Trait("Modulo", "Infrastructure")]
	[Trait("Repository", "TaskRepository")]
	public async Task GetAllByUserId_GivenOnlyCompleteFilter_ShouldReturnOnlyCompletedUserTasks()
	{
		// Arrange
		var expectedUserId = _user1.Id;

		// Act
		var actualTasks = await _uow.TaskRepository.GetAllTasksByUserIdAsync(expectedUserId, TaskFilter.OnlyComplete);

		// Assert
		actualTasks.Should().OnlyContain(t => t.UserId == expectedUserId && t.IsCompleted);
		actualTasks.Should().Contain(t => t.Id == _task2.Id);
		actualTasks.Should().Contain(t => t.Id == _todayTask2Completed.Id);
	}

	[Fact]
	[Trait("Modulo", "Infrastructure")]
	[Trait("Repository", "TaskRepository")]
	public async Task GetAllByUserId_GivenNonExistentUserId_ShouldReturnEmptyCollection()
	{
		// Arrange
		var nonExistentUserId = Guid.NewGuid();

		// Act
		var actualTasks = await _uow.TaskRepository.GetAllTasksByUserIdAsync(nonExistentUserId, TaskFilter.All);

		// Assert
		actualTasks.Should().BeEmpty();
	}

	[Fact]
	[Trait("Modulo", "Infrastructure")]
	[Trait("Repository", "TaskRepository")]
	public async Task GetSubTasksByParentTaskId_GivenParentWithSubTasks_ShouldReturnSubTasks()
	{
		// Arrange
		var parentTaskId = _task1.Id;

		// Act
		var actualSubTasks = await _uow.TaskRepository.GetSubTasksByParentTaskIdAsync(parentTaskId, TaskFilter.All);

		// Assert
		actualSubTasks.Should().HaveCount(1);
		actualSubTasks.Should().ContainSingle(t => t.Id == _subTask1.Id && t.ParentTaskId == parentTaskId);
	}

	[Fact]
	[Trait("Modulo", "Infrastructure")]
	[Trait("Repository", "TaskRepository")]
	public async Task GetSubTasksByParentTaskId_GivenParentWithoutSubTasks_ShouldReturnEmptyCollection()
	{
		// Arrange
		var parentTaskIdWithoutSubTasks = _task3.Id;

		// Act
		var actualSubTasks = await _uow.TaskRepository.GetSubTasksByParentTaskIdAsync(parentTaskIdWithoutSubTasks, TaskFilter.All);

		// Assert
		actualSubTasks.Should().BeEmpty();
	}

	[Fact]
	[Trait("Modulo", "Infrastructure")]
	[Trait("Repository", "TaskRepository")]
	public async Task GetByListId_GivenOnlyIncompleteFilter_ShouldReturnOnlyIncompleteListTasks()
	{
		// Arrange
		var listId = _list1.Id;

		// Act
		var actualTasks = await _uow.TaskRepository.GetTasksByListIdAsync(listId, TaskFilter.OnlyIncomplete);

		// Assert
		actualTasks.Should().OnlyContain(t => t.ListId == listId && !t.IsCompleted);
		actualTasks.Should().Contain(t => t.Id == _task1.Id);
		actualTasks.Should().Contain(t => t.Id == _subTask1.Id);
		actualTasks.Should().Contain(t => t.Id == _todayTask1.Id);
	}

	[Fact]
	[Trait("Modulo", "Infrastructure")]
	[Trait("Repository", "TaskRepository")]
	public async Task GetByListId_GivenOnlyCompleteFilter_ShouldReturnOnlyCompletedListTasks()
	{
		// Arrange
		var listId = _list1.Id;

		// Act
		var actualTasks = await _uow.TaskRepository.GetTasksByListIdAsync(listId, TaskFilter.OnlyComplete);

		// Assert
		actualTasks.Should().OnlyContain(t => t.ListId == listId && t.IsCompleted);
		actualTasks.Should().Contain(t => t.Id == _task2.Id);
		actualTasks.Should().Contain(t => t.Id == _todayTask2Completed.Id);
	}

	[Fact]
	[Trait("Modulo", "Infrastructure")]
	[Trait("Repository", "TaskRepository")]
	public async Task GetTodayByUserId_GivenOnlyIncompleteFilter_ShouldReturnOnlyTodayIncompleteTasks()
	{
		// Arrange
		var expectedUserId = _user1.Id;

		// Act
		var actualTasks = await _uow.TaskRepository.GetTodayTasksByUserIdAsync(expectedUserId, TaskFilter.OnlyIncomplete);

		// Assert
		actualTasks.Should().OnlyContain(t =>
			t.UserId == expectedUserId
			&& t.Scheduling != null
			&& t.Scheduling.Date == DateOnly.FromDateTime(DateTime.UtcNow)
			&& !t.IsCompleted);
		actualTasks.Should().ContainSingle(t => t.Id == _todayTask1.Id);
	}

	[Fact]
	[Trait("Modulo", "Infrastructure")]
	[Trait("Repository", "TaskRepository")]
	public async Task GetTodayByUserId_GivenOnlyCompleteFilter_ShouldReturnOnlyTodayCompletedTasks()
	{
		// Arrange
		var expectedUserId = _user1.Id;

		// Act
		var actualTasks = await _uow.TaskRepository.GetTodayTasksByUserIdAsync(expectedUserId, TaskFilter.OnlyComplete);

		// Assert
		actualTasks.Should().OnlyContain(t =>
			t.UserId == expectedUserId
			&& t.Scheduling != null
			&& t.Scheduling.Date == DateOnly.FromDateTime(DateTime.UtcNow)
			&& t.IsCompleted);
		actualTasks.Should().ContainSingle(t => t.Id == _todayTask2Completed.Id);
	}

	[Fact]
	[Trait("Modulo", "Infrastructure")]
	[Trait("Repository", "TaskRepository")]
	public async Task Create_GivenCorrectData_ShouldCreateNewTask()
	{
		// Arrange
		var expectedTask = new TaskEntity(
			_user1.Id,
			_faker.Lorem.Sentence(3),
			_faker.Lorem.Text(),
			_list1.Id,
			TaskPriority.P1
		);

		// Act
		await _uow.TaskRepository.AddAsync(expectedTask);
		await _uow.CommitAsync();
		var actualTask = await _uow.TaskRepository.GetByIdAsync(expectedTask.Id);

		// Assert
		actualTask.Should().NotBeNull();
		actualTask.Should().Be(expectedTask);
	}

	[Fact]
	[Trait("Modulo", "Infrastructure")]
	[Trait("Repository", "TaskRepository")]
	public async Task Update_GivenExistingTask_ShouldPersistChanges()
	{
		// Arrange
		var newTitle = _faker.Lorem.Sentence(5);
		var newDescription = _faker.Lorem.Text();
		_task1.UpdateDetails(newTitle, newDescription);
		_task1.UpdatePriority(TaskPriority.P1);
		_task1.MoveToList(_list2.Id);

		// Act
		await _uow.TaskRepository.UpdateAsync(_task1);
		await _uow.CommitAsync();
		var actualTask = await _uow.TaskRepository.GetByIdAsync(_task1.Id);

		// Assert
		actualTask.Should().NotBeNull();
		actualTask!.Title.Should().Be(newTitle);
		actualTask.Description.Should().Be(newDescription);
		actualTask.Priority.Should().Be(TaskPriority.P1);
		actualTask.ListId.Should().Be(_list2.Id);
	}

	[Fact]
	[Trait("Modulo", "Infrastructure")]
	[Trait("Repository", "TaskRepository")]
	public async Task Delete_GivenExistingTask_ShouldRemoveTask()
	{
		// Arrange
		var taskToDelete = _task3;

		// Act
		await _uow.TaskRepository.DeleteAsync(taskToDelete);
		await _uow.CommitAsync();
		var actualTask = await _uow.TaskRepository.GetByIdAsync(taskToDelete.Id);

		// Assert
		actualTask.Should().BeNull();
	}
    
}
