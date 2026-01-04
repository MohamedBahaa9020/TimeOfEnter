namespace TimeOfEnter.Common.Responses;

public record CancelResponse(string Message, DateTime? StartTime, DateTime? EndTime);
