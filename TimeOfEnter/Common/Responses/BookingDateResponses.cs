namespace TimeOfEnter.Common.Responses;

public record BookingDateResponses(bool IsActive, string Message, DateTime? StartTime, DateTime? EndTime);
