using System;
using FluentAssertions;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.ValueObjects;
using TimeZoneNotFoundException = TaskFlow.Domain.Exceptions.TimeZoneNotFoundException;

namespace TaskFlow.Domain.UnitTests.ValueObjects;

public class UserTimeZoneTests
{
    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Value Object", "UserTimeZone")]
    public void Constructor_GivenValidZoneId_ShouldCreateObject()
    {
        // Arrange
        const string zoneId = "UTC";

        // Act
        var userTimeZone = new UserTimeZone(zoneId);

        // Assert
        userTimeZone.ZoneId.Should().Be(zoneId);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Value Object", "UserTimeZone")]
    public void Constructor_GivenZoneIdWithSpaces_ShouldTrimZoneId()
    {
        // Arrange
        const string zoneId = " UTC ";

        // Act
        var userTimeZone = new UserTimeZone(zoneId);

        // Assert
        userTimeZone.ZoneId.Should().Be("UTC");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [Trait("Modulo", "Domain")]
    [Trait("Value Object", "UserTimeZone")]
    public void Constructor_GivenEmptyZoneId_ShouldThrowDataIsInvalidException(string? zoneId)
    {
        // Act
        var func = () => new UserTimeZone(zoneId!);

        // Assert
        var exception = func.Should().Throw<DataIsInvalidException>().Which;
        exception.Code.Should().Be("ZONE_IS_NULL");
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Value Object", "UserTimeZone")]
    public void Constructor_GivenUnknownZoneId_ShouldThrowTimeZoneNotFoundException()
    {
        // Arrange
        const string invalidZoneId = "INVALID_TZ_ID";

        // Act
        var func = () => new UserTimeZone(invalidZoneId);

        // Assert
        func.Should().Throw<TimeZoneNotFoundException>();
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Value Object", "UserTimeZone")]
    public void Default_ShouldUseUtc()
    {
        // Act
        var userTimeZone = UserTimeZone.Default;

        // Assert
        userTimeZone.ZoneId.Should().Be("UTC");
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Value Object", "UserTimeZone")]
    public void ConvertFromUtc_GivenDateThatIsNotUtc_ShouldThrowDataIsInvalidException()
    {
        // Arrange
        var userTimeZone = new UserTimeZone("UTC");
        var localDate = new DateTime(2026, 3, 20, 10, 30, 0, DateTimeKind.Local);

        // Act
        var func = () => userTimeZone.ConvertFromUtc(localDate);

        // Assert
        var exception = func.Should().Throw<DataIsInvalidException>().Which;
        exception.Code.Should().Be("DATE_IS_NOT_UTC");
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Value Object", "UserTimeZone")]
    public void ConvertFromUtc_GivenUtcDateAndUtcZone_ShouldKeepSameInstant()
    {
        // Arrange
        var userTimeZone = new UserTimeZone("UTC");
        var utcDate = new DateTime(2026, 3, 20, 10, 30, 0, DateTimeKind.Utc);

        // Act
        var convertedDate = userTimeZone.ConvertFromUtc(utcDate);

        // Assert
        convertedDate.Should().Be(utcDate);
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Value Object", "UserTimeZone")]
    public void ConvertToUtc_GivenLocalDatePartsAndUtcZone_ShouldConvertToUtcDateTime()
    {
        // Arrange
        var userTimeZone = new UserTimeZone("UTC");
        var localDate = new DateOnly(2026, 3, 20);
        var localTime = new TimeOnly(10, 30, 0);

        // Act
        var convertedDate = userTimeZone.ConvertToUtc(localDate, localTime);

        // Assert
        convertedDate.Should().Be(new DateTime(2026, 3, 20, 10, 30, 0, DateTimeKind.Utc));
    }

    [Fact]
    [Trait("Modulo", "Domain")]
    [Trait("Value Object", "UserTimeZone")]
    public void ConvertFromUtc_GivenUtcDatePartsAndUtcZone_ShouldConvertToSameDateTime()
    {
        // Arrange
        var userTimeZone = new UserTimeZone("UTC");
        var utcDate = new DateOnly(2026, 3, 20);
        var utcTime = new TimeOnly(10, 30, 0);

        // Act
        var convertedDate = userTimeZone.ConvertFromUtc(utcDate, utcTime);

        // Assert
        convertedDate.Should().Be(new DateTime(2026, 3, 20, 10, 30, 0, DateTimeKind.Utc));
    }
}
