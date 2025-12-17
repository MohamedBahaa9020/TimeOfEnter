namespace TimeOfEnter.Common.Responses
{
        public record ApiResponse<T>(bool IsSuccess, T Data);
        public record ApiResponse(bool IsSuccess, object Data);
   
}
