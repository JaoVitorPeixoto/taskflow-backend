using System;
using Bogus;
using FluentAssertions;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.ValueObjects;
using TaskFlow.Infrastructure.Persistence.Repositories;
using Task = System.Threading.Tasks.Task;
using User = TaskFlow.Domain.Entities.User;

namespace TaskFlow.Infrastructure.UnitTests.Persistence.Repositories;

public class ListRepositoryTests : IAsyncLifetime
{
    private readonly Faker _faker = new("pt_BR");

    private SqliteTestDb _db = null!;
    private UnitOfWork _uow = null!;

    private User _user1 = null!;
    private User _user2 = null!;

    private List _list1 = null!;
    private List _list2 = null!;
    

    public async Task InitializeAsync()
    {
        _db = new SqliteTestDb();
        _uow = new UnitOfWork(_db.Context);

        _user1 = new User(
            _faker.Name.FullName(),
            new Email(_faker.Internet.Email()),
            _faker.Internet.Password(),
            new AvatarUrl(_faker.Internet.Avatar()),
            new UserTimeZone("UTC")
        );

        _user2 = new User(
            _faker.Name.FullName(),
            new Email(_faker.Internet.Email()),
            _faker.Internet.Password(),
            new AvatarUrl(_faker.Internet.Avatar()),
            new UserTimeZone("UTC")
        );
        
        _list1 = new List(
            _user1.Id,
            _faker.Lorem.Sentence(6),
            _faker.Lorem.Text()
        );

        _list2 = new List(
            _user2.Id,
            _faker.Lorem.Sentence(6),
            _faker.Lorem.Text()
        );

        await _uow.UserRepository.AddAsync(_user1);
        await _uow.UserRepository.AddAsync(_user2);
        await _uow.ListRepository.AddAsync(_list1);
        await _uow.ListRepository.AddAsync(_list2);
        await _uow.CommitAsync();
    }

    public async Task DisposeAsync()
    {
        await _uow.DisposeAsync();
        await _db.DisposeAsync();
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "ListRepository")]
    public async Task Get_GivenValidListId_ShouldReturnList()
    {
        // Arrange
        var expectedList = _list1;

        // Act
        var actualList = await _uow.ListRepository.GetByIdAsync(expectedList.Id);

        // Assert
        actualList.Should().NotBeNull();
        actualList.Should().Be(expectedList);
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "ListRepository")]
    public async Task Get_GivenNonExistentListId_ShouldReturnNull()
    {
        // Arrange
        var nonExistentListId = Guid.NewGuid();

        // Act
        var actualList = await _uow.ListRepository.GetByIdAsync(nonExistentListId);

        // Assert
        actualList.Should().BeNull();
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "ListRepository")]
    public async Task GetAll_GivenExistingListsInDb_ShouldReturnListOfLists()
    {
        // Arrange
        var expectedLists = new[] { _list1, _list2 };

        // Act
        var actualLists = await _uow.ListRepository.GetAllAsync();

        // Assert
        actualLists.Should().BeEquivalentTo(expectedLists);
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "ListRepository")]
    public async Task GetAll_GivenFilter_ShouldReturnOnlyMatchedLists()
    {
        // Arrange
        var expectedTitle = _list1.Title;

        // Act
        var actualLists = await _uow.ListRepository.GetAllAsync(l => l.Title == expectedTitle);

        // Assert
        actualLists.Should().HaveCount(1);
        actualLists.Should().ContainSingle(l => l.Id == _list1.Id);
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "ListRepository")]
    public async Task GetByUserId_GivenExistingUserId_ShouldReturnOnlyUserLists()
    {
        // Arrange
        var expectedUserId = _user1.Id;

        // Act
        var actualLists = await _uow.ListRepository.GetLitsByUserIdAsync(expectedUserId);

        // Assert
        actualLists.Should().HaveCount(1);
        actualLists.Should().ContainSingle(l => l.Id == _list1.Id && l.UserId == expectedUserId);
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "ListRepository")]
    public async Task GetByUserId_GivenNonExistentUserId_ShouldReturnEmptyCollection()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var actualLists = await _uow.ListRepository.GetLitsByUserIdAsync(nonExistentUserId);

        // Assert
        actualLists.Should().BeEmpty();
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "ListRepository")]
    public async Task Create_GivenCorrectData_ShouldCreateNewList()
    {
        // Arrange
        var expectedList = new List(
            _user1.Id,
            _faker.Lorem.Sentence(6),
            _faker.Lorem.Text()
        );

        // Act
        await _uow.ListRepository.AddAsync(expectedList);
        await _uow.CommitAsync();
        var actualList = await _uow.ListRepository.GetByIdAsync(expectedList.Id);

        // Assert
        actualList.Should().NotBeNull();
        actualList.Should().Be(expectedList);
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "ListRepository")]
    public async Task Update_GivenExistingList_ShouldPersistChanges()
    {
        // Arrange
        var newTitle = _faker.Lorem.Sentence(4);
        var newDescription = _faker.Lorem.Text();
        _list1.UpdateDetails(newTitle, newDescription);

        // Act
        await _uow.ListRepository.UpdateAsync(_list1);
        await _uow.CommitAsync();
        var actualList = await _uow.ListRepository.GetByIdAsync(_list1.Id);

        // Assert
        actualList.Should().NotBeNull();
        actualList!.Title.Should().Be(newTitle);
        actualList.Description.Should().Be(newDescription);
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "ListRepository")]
    public async Task Delete_GivenExistingList_ShouldRemoveList()
    {
        // Arrange
        var listToDelete = _list2;

        // Act
        await _uow.ListRepository.DeleteAsync(listToDelete);
        await _uow.CommitAsync();
        var actualList = await _uow.ListRepository.GetByIdAsync(listToDelete.Id);

        // Assert
        actualList.Should().BeNull();
    }

}
