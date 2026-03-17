using Bogus;
using FluentAssertions;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.ValueObjects;

namespace TaskFlow.Domain.UnitTests.Entities;

public class UserTests
{
    private readonly Faker _faker = new Faker("pt_BR");

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "User")]
    public void Constructor_GivenValidData_ShouldCreateUser()
    {
        // Arrange
        var name = _faker.Person.FullName;
        var email = new Email(_faker.Internet.Email());
        var password = _faker.Internet.Password();
        var avatarUrl = new AvatarUrl(_faker.Internet.Avatar());

        // Act
        var user = new User(name, email, password, avatarUrl);

        // Assert
        Assert.Equal(name, user.Name);
        Assert.Equal(email, user.Email);
        Assert.Equal(password, user.Password);
        Assert.Equal(avatarUrl, user.AvatarUrl);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "User")]
    public void Constructor_GivenEmptyName_ShouldThrowDataIsInvalidException()
    {
        // Arrange
        var email = new Email(_faker.Internet.Email());
        var password = _faker.Internet.Password();
        var avatarUrl = new AvatarUrl(_faker.Internet.Avatar());

        // Act & Assert
        var exception = Assert.Throws<DataIsInvalidException>(() => new User("", email, password, avatarUrl));
        Assert.Equal("NAME_IS_EMPTY", exception.Code);
    }

    
    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "User")]
    public void Constructor_GivenEmptyPassword_ShouldThrowDataIsInvalidException()
    {
        // Arrange
        var name = _faker.Person.FullName;
        var email = new Email(_faker.Internet.Email());
        var avatarUrl = new AvatarUrl(_faker.Internet.Avatar());

        // Act & Assert
        var exception = Assert.Throws<DataIsInvalidException>(() => new User(name, email, "", avatarUrl));
        Assert.Equal("PASSWORD_IS_EMPTY", exception.Code);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "User")]
    public void UpdateName_GivenValidName_ShouldUpdateName()
    {
        // Arrange
        var user = CreateValidUser();
        var newName = _faker.Person.FullName;

        // Act
        user.UpdateName(newName);

        // Assert
        Assert.Equal(newName, user.Name);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "User")]
    public void UpdateName_GivenEmptyName_ShouldThrowDataIsInvalidException()
    {
        // Arrange
        var user = CreateValidUser();

        // Act & Assert
        var exception = Assert.Throws<DataIsInvalidException>(() => user.UpdateName(""));
        Assert.Equal("NAME_IS_EMPTY", exception.Code);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "User")]
    public void UpdatePassword_GivenValidPassword_ShouldUpdatePassword()
    {
        // Arrange
        var user = CreateValidUser();
        var newPassword = _faker.Internet.Password();

        // Act
        user.UpdatePassword(newPassword);

        // Assert
        Assert.Equal(newPassword, user.Password);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "User")]
    public void UpdatePassword_GivenEmptyPassword_ShouldThrowDataIsInvalidException()
    {
        // Arrange
        var user = CreateValidUser();

        // Act & Assert
        var exception = Assert.Throws<DataIsInvalidException>(() => user.UpdatePassword(""));
        Assert.Equal("PASSWORD_IS_EMPTY", exception.Code);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "User")]
    public void UpdateEmail_GivenValidEmail_ShouldUpdateEmail()
    {
        // Arrange
        var user = CreateValidUser();
        var newEmail = new Email(_faker.Internet.Email());

        // Act
        user.UpdateEmail(newEmail);

        // Assert
        Assert.Equal(newEmail, user.Email);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "User")]
    public void UpdateAvatarUrl_GivenValidAvatarUrl_ShouldUpdateAvatarUrl()
    {
        // Arrange
        var user = CreateValidUser();
        var newAvatarUrl = new AvatarUrl(_faker.Internet.Avatar());

        // Act
        user.UpdateAvatarUrl(newAvatarUrl);

        // Assert
        Assert.Equal(newAvatarUrl, user.AvatarUrl);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "User")]
    public void Compare_GivenTwoEqualsUsers_ShouldBeEquals()
    {
        // Arrange
        var commonUser = CreateValidUser();
        var user1 = commonUser;
        var user2 = commonUser;

        // Act and Assert
        user1.Should().Be(user2);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "User")]
    public void Compare_GivenTwoDiferentUsers_ShouldBeDiferents()
    {
        // Arrange
        var user1 = new User("João Peixoto", new("joao1@teste.com"), "1234", new AvatarUrl(_faker.Internet.Avatar()));
        var user2 = new User("João Peixoto 2", new("joao2@teste.com"), "1234", new AvatarUrl(_faker.Internet.Avatar()));

        // Act and Assert
        user1.Should().NotBe(user2);
    }

    private User CreateValidUser()
    {
        return new User(
            _faker.Person.FullName,
            new Email(_faker.Internet.Email()),
            _faker.Internet.Password(),
            new AvatarUrl(_faker.Internet.Avatar())
        );
    }
}
