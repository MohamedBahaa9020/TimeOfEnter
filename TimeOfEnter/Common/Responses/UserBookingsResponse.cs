namespace TimeOfEnter.Common.Responses;

public record UserBookingsResponse(bool IsActive, DateTime? StartTime, DateTime? EndTime);

