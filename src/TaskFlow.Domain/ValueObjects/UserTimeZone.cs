using System;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Domain.ValueObjects;

public record UserTimeZone
{
    public string ZoneId { get; private set; } = null!;

    public static UserTimeZone Default { get; } = new("UTC");

    // EF Core
    private UserTimeZone() {}

    public UserTimeZone(string zoneId)
    {
        zoneId = zoneId?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(zoneId))
            throw new DataIsInvalidException("ZONE_IS_NULL", "Time Zone cannot be null.");

        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(zoneId);

            ZoneId = zoneId;
        }
        catch (System.TimeZoneNotFoundException)
        {
            throw new TaskFlow.Domain.Exceptions.TimeZoneNotFoundException(zoneId);
        }
        catch (InvalidTimeZoneException)
        {
            throw new TaskFlow.Domain.Exceptions.TimeZoneNotFoundException(zoneId);
        }

    }

    public DateTime ConvertFromUtc(DateTime utcDate)
    {
        if (utcDate.Kind != DateTimeKind.Utc)
            throw new DataIsInvalidException("DATE_IS_NOT_UTC", "Date must be in UTC.");

        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utcDate, ZoneId);
    }

    public DateTime ConvertFromUtc(DateOnly utcDate, TimeOnly utcTime)
    {
        var utcDateTime = DateTime.SpecifyKind(utcDate.ToDateTime(utcTime), DateTimeKind.Utc);
        return ConvertFromUtc(utcDateTime);
    }

    public DateTime ConvertToUtc(DateTime localDate)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById(ZoneId);
        return TimeZoneInfo.ConvertTimeToUtc(localDate, tz);
    }

    public DateTime ConvertToUtc(DateOnly localDate, TimeOnly localTime)
    {
        var localDateTime = DateTime.SpecifyKind(localDate.ToDateTime(localTime), DateTimeKind.Unspecified);
        return ConvertToUtc(localDateTime);
    }
}

