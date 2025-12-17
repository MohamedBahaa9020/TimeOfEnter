namespace TimeOfEnter.Common.Pagination
{
    public sealed record PageResult<T>(
     int Page,
     int PageSize,
     int TotalPages,
     int TotalItems,
     List<T> Items
 );
}
