namespace TimeOfEnter.Common.Responses;

public record BookingDateResponse(bool IsActive, string Message, DateTime? StartTime, DateTime? EndTime);
