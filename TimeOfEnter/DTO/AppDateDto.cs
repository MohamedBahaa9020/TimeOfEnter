namespace TimeOfEnter.DTO;

public sealed record AppDateDto(
  int Id,
  DateTime StartTime,
  DateTime? EndTime,
  bool IsActive
);
