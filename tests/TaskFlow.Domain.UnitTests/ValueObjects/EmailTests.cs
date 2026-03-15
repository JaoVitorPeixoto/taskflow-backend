using System.Net.Mail;
using Bogus;
using FluentAssertions;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.ValueObjects;


namespace TaskFlow.Domain.UnitTests.ValueObjects;

public class EmailTests
{

    private readonly Faker _faker = new Faker("pt_BR");
    

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Value Object", "Email")]
    public void Constructor_GivenCorrectData_ShouldReturnNewObjectEmail()
    {
        // Arrange
        var expectedEmail = _faker.Internet.Email();

        // Act
        var email = new Email(expectedEmail);

        // Assert
        email.Address.Should().Be(expectedEmail.ToLower());
    }   

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("joao")]
    [InlineData("joao@")]
    [InlineData("123")]
    [InlineData("@#")]
    [Trait("Modulo", "Domain")]
    [Trait("Value Object", "Email")]
    public void Constructor_GivenIncorrectData_ShouldThrowsDataIsInvalidException(string? emailTest)
    {
        // Act
        var funcEmail = () => new Email(emailTest);

        // Assert
        funcEmail.Should().Throw<DataIsInvalidException>();
    } 


    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("ValueObject", "Email")]
    public void GivenEmail_ShouldeBeCorrectFormat()
    {
        // Arrange
        var email = new Email(_faker.Internet.Email());
 
        // Act
        var funcMail = () => new MailAddress(email.Address);

        // Assert
        funcMail.Should().NotThrow();
        email.Address.Should().Be(funcMail().Address);
    }


    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Value Object", "Email")]
    public void Compare_GivenTwoEqualsEmails_ShouldBeSameValues()
    {
        // Arrange
        var commomEmail = _faker.Internet.Email();
        var email1 = new Email(commomEmail);
        var email2 = new Email(commomEmail);

        // Act and Assert
        email1.Should().Be(email2);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Value Object", "Email")]
    public void Compare_GivenTwoDiferentEmails_ShouldBeDiferent()
    {
        // Arrange
        var email1 = new Email("exemplo1@exemplo.com");
        var email2 = new Email("exemplo2@exemplo.com");

        // Act and Assert
        email1.Should().NotBe(email2);
    }

}
