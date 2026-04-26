namespace Application.Common
{
    public class Result
    {
        protected bool success;
        protected string? error;
        public bool Success => success;
        public bool Failed => !success;
        public string? ErrorMessage => error;
        protected Result(bool success, string? error)
        {
            this.success = success;
            this.error = error;
        }
        public static Result Ok() => new Result(true, null);
        public static Result Fail(string error = "") => new Result(false, error);
        public static Result<T> Ok<T>(T data) => Result<T>.Ok(data);
    }
    public class Result<T> : Result
    {
        protected T? data;

        public T? Data => success ? data : default;

        protected Result(bool success, T? data, string? error) : base(success, error)
        {
            this.data = data;
        }
        public static Result<T> Ok(T data) => new Result<T>(true, data, null);
        public new static Result<T> Fail(string error = "")
        => new Result<T>(false, default, error);
    }
}
