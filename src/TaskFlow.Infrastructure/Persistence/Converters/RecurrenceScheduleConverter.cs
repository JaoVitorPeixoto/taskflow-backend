using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Infrastructure.Persistence.Configurations.Converters;

public sealed class RecurrenceScheduleConverter : ValueConverter<RecurrenceSchedule, string>
{
    public RecurrenceScheduleConverter()
        : base(
            toDb => ToDb(toDb),
            fromDb => FromDb(fromDb))
    {
    }

    private static string ToDb(RecurrenceSchedule value) => value switch
    {
        RecurrenceSchedule.None => "NENHUMA",
        RecurrenceSchedule.Daily => "DIARIA",
        RecurrenceSchedule.Weekly => "SEMANAL",
        RecurrenceSchedule.Monthly => "MENSAL",
        RecurrenceSchedule.Annual => "ANUAL",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };

    private static RecurrenceSchedule FromDb(string value) => (value ?? string.Empty).ToUpperInvariant() switch
    {
        "NENHUMA" => RecurrenceSchedule.None,
        "DIARIA" => RecurrenceSchedule.Daily,
        "SEMANAL" => RecurrenceSchedule.Weekly,
        "MENSAL" => RecurrenceSchedule.Monthly,
        "ANUAL" => RecurrenceSchedule.Annual,
        _ => throw new InvalidOperationException($"Invalid Recurrence: {value}")
    };
}