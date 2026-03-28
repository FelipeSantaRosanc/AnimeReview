namespace AnimeReview.Shared;

public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public string Error { get; }
    public int StatusCode { get; }

    private Result(bool isSuccess, T? value, string error, int statusCode)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        StatusCode = statusCode;
    }

    public static Result<T> Success(T value) =>
        new(true, value, string.Empty, 200);

    public static Result<T> Created(T value) =>
        new(true, value, string.Empty, 201);

    public static Result<T> Failure(string error, int statusCode = 400) =>
        new(false, default, error, statusCode);

    public static Result<T> NotFound(string message = "Resource not found") =>
        new(false, default, message, 404);

    public static Result<T> BadRequest(string message) =>
        new(false, default, message, 400);

    public static Result<T> Unauthorized(string message = "Unauthorized") =>
        new(false, default, message, 401);

    public static Result<T> Conflict(string message) =>
        new(false, default, message, 409);

    public static Result<T> Forbidden(string message = "Forbidden") =>
        new(false, default, message, 403);
}

// Result sem tipo para void
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
    public int StatusCode { get; }

    private Result(bool isSuccess, string error, int statusCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        StatusCode = statusCode;
    }

    public static Result Success() => new(true, string.Empty, 200);
    public static Result NoContent() => new(true, string.Empty, 204);
    public static Result Failure(string error, int statusCode = 400) =>
        new(false, error, statusCode);
    public static Result NotFound(string message = "Resource not found") =>
        new(false, message, 404);
    public static Result BadRequest(string message) =>
        new(false, message, 400);
    public static Result Unauthorized(string message = "Unauthorized") =>
        new(false, message, 401);
    public static Result Conflict(string message) =>
        new(false, message, 409);
    public static Result Forbidden(string message = "Forbidden") =>
        new(false, message, 403);
}