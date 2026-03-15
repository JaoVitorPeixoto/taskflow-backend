using FluentAssertions;

using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.ValueObjects;

namespace TaskFlow.Domain.UnitTests.ValueObjects;

public class SchedulingOnceTests
{

    [Fact]
    [Trait("Module", "Domain")]
    [Trait("ValueObject", "Scheduling")]
    public void Constructor_GivenCorrectData_ShouldReturnNewObjectSchedulingWithoutTime()
    {
        // Arrange
        var expectedDate = DateOnly.FromDateTime(DateTime.Today);
        TimeOnly? expectedTime = null;
        var expectedRecurrence = RecurrenceSchedule.None;

        // Act
        var scheduling = Scheduling.Factory.Once(expectedDate);
    
        // Assert
        scheduling.Date.Should().Be(expectedDate);
        scheduling.Time.Should().Be(expectedTime);
        scheduling.Recurrence.Should().Be(expectedRecurrence);
    }
    

    [Fact]
    [Trait("Module", "Domain")]
    [Trait("ValueObject", "Scheduling")]
    public void Constructor_GivenCorrectData_ShouldReturnNewObjectSchedulingWithTime()
    {
        // Arrange
        var expectedDate = DateOnly.FromDateTime(DateTime.Today);
        TimeOnly? expectedTime = TimeOnly.FromDateTime(DateTime.Now);
        var expectedRecurrence = RecurrenceSchedule.None;

        // Act
        var scheduling = Scheduling.Factory.Once(expectedDate, expectedTime);
    
        // Assert
        scheduling.Date.Should().Be(expectedDate);
        scheduling.Time.Should().Be(expectedTime);
        scheduling.Recurrence.Should().Be(expectedRecurrence);
    }

    [Fact]
    [Trait("Module", "Domain")]
    [Trait("ValueObject", "Scheduling")]
    public void Constructor_GivenDateInPast_ShouldThrowDataIsInvalidException()
    {
        // Arrange
        var pastDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        // Act
        Action act = () => Scheduling.Factory.Once(pastDate);

        // Assert
        act.Should().Throw<DataIsInvalidException>();
    }


    [Fact]
    [Trait("Module", "Domain")]
    [Trait("ValueObject", "Scheduling")]
    public void Constructor_GivenFarFutureDate_ShouldCreateScheduling()
    {
        // Arrange
        var futureDate = DateOnly.FromDateTime(DateTime.Today.AddYears(100));
        TimeOnly? expectedTime = null;
        var expectedRecurrence = RecurrenceSchedule.None;

        // Act
        var scheduling = Scheduling.Factory.Once(futureDate);

        // Assert
        scheduling.Date.Should().Be(futureDate);
        scheduling.Time.Should().Be(expectedTime);
        scheduling.Recurrence.Should().Be(expectedRecurrence);
    }
}
