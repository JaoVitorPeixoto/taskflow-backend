using Bogus;
using FluentAssertions;
using TaskFlow.Domain.ValueObjects;
using TaskFlow.Infrastructure.Persistence.Repositories;
using Task = System.Threading.Tasks.Task;
using User = TaskFlow.Domain.Entities.User;

namespace TaskFlow.Infrastructure.UnitTests.Persistence.Repositories;

public class UserRepositoryTests : IAsyncLifetime
{
    private readonly Faker _faker = new("pt_BR");

    private SqliteTestDb _db = null!;
    private UnitOfWork _uow = null!;

    private User _user1 = null!;
    private User _user2 = null!;

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

        await _uow.UserRepository.AddAsync(_user1);
        await _uow.UserRepository.AddAsync(_user2);
        await _uow.CommitAsync();
    }

    public async Task DisposeAsync()
    {
        await _uow.DisposeAsync();
        await _db.DisposeAsync();
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "UserRepository")]
    public async Task Get_GivenValidUserId_ShouldReturnUser()
    {
        // Arrange
        var expectedUser = _user1;
    
        // Act
        var actualUser = await _uow.UserRepository.GetByIdAsync(expectedUser.Id);
    
        // Assert
        actualUser.Should().NotBeNull();
        actualUser.Should().Be(expectedUser);
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "UserRepository")]
    public async Task Get_GivenNonExistentUserId_ShouldReturnNullUser()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var actualUser = await _uow.UserRepository.GetByIdAsync(nonExistentUserId);

        // Assert
        actualUser.Should().BeNull();
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "UserRepository")]
    public async Task GetAll_GivenExistingUsersInDb_ShouldReturnListOfUser()
    {
        // Arrange
        var expectedUsers = new List<User> { _user1, _user2 };

        // Act
        var actualUsers = await _uow.UserRepository.GetAllAsync();

        // Assert
        actualUsers.Should().BeEquivalentTo(expectedUsers);
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "UserRepository")]
    public async Task GetAll_GivenFilter_ShouldReturnOnlyMatchedUsers()
    {
        // Arrange
        var expectedEmail = _user1.Email;

        // Act
        var actualUsers = await _uow.UserRepository.GetAllAsync(u => u.Email.Address == expectedEmail.Address);

        // Assert
        actualUsers.Should().HaveCount(1);
        actualUsers.Should().ContainSingle(u => u.Id == _user1.Id);
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "UserRepository")]
    public async Task GetByEmail_GivenExistingEmail_ShouldReturnUser()
    {
        // Arrange
        var expectedUser = _user1;

        // Act
        var actualUser = await _uow.UserRepository.GetByEmailAsync(expectedUser.Email);

        // Assert
        actualUser.Should().NotBeNull();
        actualUser.Should().Be(expectedUser);
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "UserRepository")]
    public async Task GetByEmail_GivenNonExistingEmail_ShouldReturnNull()
    {
        // Arrange
        var nonExistingEmail = new Email(_faker.Internet.Email());

        // Act
        var actualUser = await _uow.UserRepository.GetByEmailAsync(nonExistingEmail);

        // Assert
        actualUser.Should().BeNull();
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "UserRepository")]
    public async Task EmailExists_GivenExistingEmail_ShouldReturnTrue()
    {
        // Arrange
        var existingEmail = _user1.Email;

        // Act
        var exists = await _uow.UserRepository.EmailExistsAsync(existingEmail);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "UserRepository")]
    public async Task EmailExists_GivenNonExistingEmail_ShouldReturnFalse()
    {
        // Arrange
        var nonExistingEmail = new Email(_faker.Internet.Email());

        // Act
        var exists = await _uow.UserRepository.EmailExistsAsync(nonExistingEmail);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "UserRepository")]
    public async Task Create_GivenCorrectData_ShouldCreateNewUser()
    {
        // Arrange
        var expectedUser = new User(
            _faker.Name.FullName(),
            new Email(_faker.Internet.Email()),
            _faker.Internet.Password(),
            new AvatarUrl(_faker.Internet.Avatar()),
            new UserTimeZone("UTC")
        );
    
        // Act
        await _uow.UserRepository.AddAsync(expectedUser);
        await _uow.CommitAsync();

        var actualUser = await _uow.UserRepository.GetByIdAsync(expectedUser.Id);
    
        // Assert
        actualUser.Should().NotBeNull();
        actualUser.Should().Be(expectedUser);
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "UserRepository")]
    public async Task Update_GivenExistingUser_ShouldPersistChanges()
    {
        // Arrange
        var newName = _faker.Name.FullName();
        var newPassword = _faker.Internet.Password();
        var newAvatarUrl = new AvatarUrl(_faker.Internet.Avatar());
        var newEmail = new Email(_faker.Internet.Email());

        _user1.UpdateName(newName);
        _user1.UpdatePassword(newPassword);
        _user1.UpdateAvatarUrl(newAvatarUrl);
        _user1.UpdateEmail(newEmail);

        // Act
        await _uow.UserRepository.UpdateAsync(_user1);
        await _uow.CommitAsync();
        var actualUser = await _uow.UserRepository.GetByIdAsync(_user1.Id);

        // Assert
        actualUser.Should().NotBeNull();
        actualUser!.Name.Should().Be(newName);
        actualUser.Password.Should().Be(newPassword);
        actualUser.AvatarUrl.Should().Be(newAvatarUrl);
        actualUser.Email.Should().Be(newEmail);
    }

    [Fact]
    [Trait("Modulo", "Infrastructure")]
    [Trait("Repository", "UserRepository")]
    public async Task Delete_GivenExistingUser_ShouldRemoveUser()
    {
        // Arrange
        var userToDelete = _user2;

        // Act
        await _uow.UserRepository.DeleteAsync(userToDelete);
        await _uow.CommitAsync();
        var actualUser = await _uow.UserRepository.GetByIdAsync(userToDelete.Id);

        // Assert
        actualUser.Should().BeNull();
    }

}
