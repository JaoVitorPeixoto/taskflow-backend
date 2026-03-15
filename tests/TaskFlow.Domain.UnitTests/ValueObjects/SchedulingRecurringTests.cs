using System;
using FluentAssertions;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.ValueObjects;

namespace TaskFlow.Domain.UnitTests.ValueObjects;

public class SchedulingRecurringTests
{

    [Fact]
    [Trait("Module", "Domain")]
    [Trait("ValueObject", "Scheduling")]
    public void Constructor_GivenValidData_ShouldCreateRecurringScheduling()
    {
        // Arrange
        var expectedDate = DateOnly.FromDateTime(DateTime.Today);
        TimeOnly? expectedTime = TimeOnly.FromDateTime(DateTime.Now);
        var expectedRecurrence = RecurrenceSchedule.Daily;

        // Act
        var scheduling = Scheduling.Factory.Recurring(expectedDate, expectedTime, expectedRecurrence);

        // Assert
        scheduling.Date.Should().Be(expectedDate);
        scheduling.Time.Should().Be(expectedTime);
        scheduling.Recurrence.Should().Be(expectedRecurrence);
    }

    [Fact]
    [Trait("Module", "Domain")]
    [Trait("ValueObject", "Scheduling")]
    public void Constructor_GivenNoneRecurrence_ShouldThrowDataIsInvalidException()
    {
        // Arrange
        var validDate = DateOnly.FromDateTime(DateTime.Today);
        TimeOnly? validTime = TimeOnly.FromDateTime(DateTime.Now);

        // Act
        Action act = () => Scheduling.Factory.Recurring(validDate, validTime, RecurrenceSchedule.None);

        // Assert
        act.Should().Throw<DataIsInvalidException>();
    }

    [Fact]
    [Trait("Module", "Domain")]
    [Trait("ValueObject", "Scheduling")]
    public void Constructor_GivenFarFutureDate_ShouldCreateRecurringScheduling()
    {
        // Arrange
        var futureDate = DateOnly.FromDateTime(DateTime.Today.AddYears(100));
        TimeOnly? expectedTime = TimeOnly.FromDateTime(DateTime.Now);
        var expectedRecurrence = RecurrenceSchedule.Weekly;

        // Act
        var scheduling = Scheduling.Factory.Recurring(futureDate, expectedTime, expectedRecurrence);

        // Assert
        scheduling.Date.Should().Be(futureDate);
        scheduling.Time.Should().Be(expectedTime);
        scheduling.Recurrence.Should().Be(expectedRecurrence);
    }
}
