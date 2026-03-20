using System;

namespace TaskFlow.Domain.Exceptions;

public class TimeZoneNotFoundException : DomainException
{

    public TimeZoneNotFoundException(string zoneId)
    : base("TIME_ZONE_NOT_FOUND", $"Time Zone not found by id {zoneId}.")
    {
        
    }
}
