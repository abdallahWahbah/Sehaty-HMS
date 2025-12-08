namespace Sehaty.Application.Services
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public T Data { get; set; }

        public static ServiceResult<T> Fail(string error) =>
            new()
            { Success = false, Error = error };

        public static ServiceResult<T> Ok(T data) =>
            new() { Success = true, Data = data };

    }
}
