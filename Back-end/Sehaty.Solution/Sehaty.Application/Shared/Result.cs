namespace Sehaty.Application.Shared
{
    public enum ErrorType
    {
        None,
        NotFound,
        Validation,
        Conflict,
        Unauthorized,
        Forbidden,
        BadRequest
    }

    public class Result
    {
        public bool IsSuccess { get; }
        public string Error { get; }
        public ErrorType ErrorType { get; }

        protected Result(bool success, string error, ErrorType errorType)
        {
            IsSuccess = success;
            Error = error;
            ErrorType = errorType;
        }

        public static Result Success()
            => new(true, null, ErrorType.None);

        public static Result Failure(ErrorType type, string error)
            => new(false, error, type);
    }

    public class Result<T> : Result
    {
        public T Data { get; }

        protected Result(bool success, T data, string error, ErrorType errorType)
            : base(success, error, errorType)
        {
            Data = data;
        }

        public static Result<T> Success(T data)
            => new(true, data, null, ErrorType.None);

        public new static Result<T> Failure(ErrorType type, string error)
            => new(false, default, error, type);
    }


}
