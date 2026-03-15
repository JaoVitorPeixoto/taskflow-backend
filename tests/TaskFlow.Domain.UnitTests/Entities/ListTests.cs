using Bogus;
using FluentAssertions;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Domain.UnitTests.Entities;

public class ListTests
{
    private readonly Faker _faker = new Faker("pt_BR");

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "List")]
    public void Constructor_GivenValidData_ShouldCreateList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var title = _faker.Lorem.Sentence();
        var description = _faker.Lorem.Paragraph();

        // Act
        var list = new List(userId, title, description);

        // Assert
        list.UserId.Should().Be(userId);
        list.Title.Should().Be(title);
        list.Description.Should().Be(description);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "List")]
    public void Constructor_GivenEmptyTitle_ShouldThrowDataIsInvalidException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var description = _faker.Lorem.Paragraph();

        // Act
        Action act = () => new List(userId, "", description);

        // Assert
        act.Should().Throw<DataIsInvalidException>()
            .WithMessage("Title cannot be empty.")
            .And.Code.Should().Be("TITLE_IS_EMPTY");
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "List")]
    public void UpdateDetails_GivenValidData_ShouldUpdateTitleAndDescription()
    {
        // Arrange
        var list = CreateValidList();
        var newTitle = _faker.Lorem.Sentence();
        var newDescription = _faker.Lorem.Paragraph();

        // Act
        list.UpdateDetails(newTitle, newDescription);

        // Assert
        list.Title.Should().Be(newTitle);
        list.Description.Should().Be(newDescription);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "List")]
    public void UpdateDetails_GivenEmptyTitle_ShouldThrowDataIsInvalidException()
    {
        // Arrange
        var list = CreateValidList();

        // Act
        Action act = () => list.UpdateDetails("", _faker.Lorem.Paragraph());

        // Assert
        act.Should().Throw<DataIsInvalidException>()
            .WithMessage("Title cannot be empty.")
            .And.Code.Should().Be("TITLE_IS_EMPTY");
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "List")]
    public void Compare_GivenTwoEqualLists_ShouldBeEqual()
    {
        // Arrange
        var commonList = CreateValidList();
        var list1 = commonList;
        var list2 = commonList;

        // Act & Assert
        list1.Should().Be(list2);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Entity", "List")]
    public void Compare_GivenTwoDifferentLists_ShouldNotBeEqual()
    {
        // Arrange
        var list1 = new List(Guid.NewGuid(), "List 1", _faker.Lorem.Paragraph());
        var list2 = new List(Guid.NewGuid(), "List 2", _faker.Lorem.Paragraph());

        // Act & Assert
        list1.Should().NotBe(list2);
    }

    private List CreateValidList()
    {
        return new List(
            Guid.NewGuid(),
            _faker.Lorem.Sentence(),
            _faker.Lorem.Paragraph()
        );
    }
}
