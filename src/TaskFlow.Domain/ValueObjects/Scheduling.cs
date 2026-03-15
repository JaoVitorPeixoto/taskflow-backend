using System;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Domain.ValueObjects;

public record Scheduling
{
    public DateOnly Date { get; private set; }
    public TimeOnly? Time { get; private set; }
    public RecurrenceSchedule Recurrence { get; private set; }
    public bool Notify {get; private set; }
    public bool IsRecurring => Recurrence != RecurrenceSchedule.None;

    private Scheduling(
        DateOnly date, 
        TimeOnly? time, 
        RecurrenceSchedule recurrenceSchedule,
        bool notify)
    {
        if (date < DateOnly.FromDateTime(DateTime.Today))
            throw new DataIsInvalidException("DATE_IN_PAST", "Date cannot be in the past.");

        if (notify == true && time == null)
            throw new DataIsInvalidException("NOTIFY_WITH_RECURRING_SCHEDULE_IS_NONE", "In order to issue a notification, the time cannot be null.");

        this.Date = date;
        this.Time = time;
        this.Recurrence = recurrenceSchedule;
        this.Notify = notify;
    }
    
    public static class Factory
    {
        public static Scheduling Once(DateOnly date, TimeOnly? time = null)
        {
            return new Scheduling(
                date, 
                time, 
                RecurrenceSchedule.None,
                false
            );
        }

        public static Scheduling Recurring(DateOnly date, TimeOnly? time = null, RecurrenceSchedule recurrence = RecurrenceSchedule.Daily, bool notify = false)
        {
            if (recurrence == RecurrenceSchedule.None)
                throw new DataIsInvalidException("RECURRING_SCHEDULE_IS_NONE", "Recurring cannot be None.");

            return new Scheduling(
                date,
                time,
                recurrence,
                notify
            );
        }
    }

}
