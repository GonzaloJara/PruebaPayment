using Serilog;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace PruebaPayment.CommonModels;

public class Result
{
    public required bool IsSuccess { get; init; }
    public bool IsError => !IsSuccess;
    public Error? Error { get; init; } = null;
    public List<KeyValuePair<string, string>> Metadata { get; init; } = [];

    private protected Result() { }

    public static Result Success()
    {
        return new Result()
        {
            IsSuccess = true,
        };
    }
    public static Result Failure(Error error, [CallerMemberName] string callerMemberName = "")
    {
        Log.Error("An error occured. CallerMemberName='{callerMemberName}'. Error: '{errorJson}'", callerMemberName, JsonSerializer.Serialize(error));

        return new Result()
        {
            IsSuccess = false,
            Error = error
        };
    }

    public static Result ValidationFailure(Error error, List<ValidationError> errors, [CallerMemberName] string callerMemberName = "")
    {
        Log.Error("An error occured. CallerMemberName='{callerMemberName}'. Error: '{errorJson}'", callerMemberName, JsonSerializer.Serialize(error));
        return new Result()
        {
            IsSuccess = false,
            Error = error,
            Metadata = [.. errors.Select(x => new KeyValuePair<string, string>(x.PropertyName, x.ErrorMessage))]
        };
    }

    public Result WithMetadata(string key, string value)
    {
        Metadata.Add(new(key, value));
        return this;
    }

    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
}

public class Result<T> : Result
{
    public T? Content { get; init; }

    private Result() { }

    public static Result<T> Success(T value)
    {
        return new Result<T>()
        {
            IsSuccess = true,
            Content = value
        };
    }

    public new static Result<T> Failure(Error error, [CallerMemberName] string callerMemberName = "")
    {
        Log.Error("An error occured. CallerMemberName='{callerMemberName}'. Error: '{errorJson}'", callerMemberName, JsonSerializer.Serialize(error));        
        return new Result<T>()
        {
            IsSuccess = false,
            Error = error
        };
    }
}

public sealed record Error(string Code, string Description);