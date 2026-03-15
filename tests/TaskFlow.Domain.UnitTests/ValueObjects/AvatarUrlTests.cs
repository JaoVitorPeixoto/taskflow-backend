using System;
using Bogus;
using FluentAssertions;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.ValueObjects;

namespace TaskFlow.Domain.UnitTests.ValueObjects;

public class AvatarUrlTests
{
    private readonly Faker _faker = new Faker("pt_BR");


    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("ValueObject", "AvatarUrl")]
    public void Constructor_GivenCorrectData_ShouldCreateNewObject()
    {
        // Arrange
        var excpectedAvatarUrl = _faker.Internet.Avatar();

        // Act
        var avatarUrl = new AvatarUrl(excpectedAvatarUrl);

        // Assert
        avatarUrl.Url.Should().Be(excpectedAvatarUrl);
    }


    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("testando")]
    [InlineData("123")]
    [InlineData("@#")]
    public void Constructor_GivenIncorrectData_ShouldThrowDataIsInvalidException(string? urlTests)
    {
        // Act
        var funcAvatarUrl = () => new AvatarUrl(urlTests);

        // Assert
        funcAvatarUrl.Should().Throw<DataIsInvalidException>();
    }


    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("ValueObject", "AvatarUrl")]
    public void Compare_GivenTwoEqualsAvatarUrls_ShouldBeSameValues()
    {
        // Arrange
        var commomUrl = _faker.Internet.Avatar();
        var url1 = new AvatarUrl(commomUrl);
        var url2 = new AvatarUrl(commomUrl);

        // Act and Assert
        url1.Should().Be(url2);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("ValueObject", "AvatarUrl")]
    public void Compare_GivenTwoDiferentAvatarUrls_ShouldBeDiferent()
    {
        // Arrange
        var url1 = new AvatarUrl("https://gravatar.com/avatar/hashteste01");
        var url2 = new AvatarUrl("https://gravatar.com/avatar/hashteste02");

        // Act and Assert
        url1.Should().NotBe(url2);
    }
}
