namespace TimeOfEnter.Common.Responses;

public record ApiResponse<T>(bool IsSuccess, T Data);