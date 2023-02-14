using Microsoft.Extensions.Logging;
using System.Collections;

namespace rhz_result;
public interface IErrorable {
    public bool IsSuccess { get; }
    public bool IsError { get; }
    public Exception Error { get; }
}
public interface IValue<T> {
    public T Value { get; }
}
public interface IResult<T> : IErrorable, IValue<T> { }

public readonly struct Result<TValue> : IResult<TValue> {
    private readonly TValue? value;
    private readonly Exception? error;
    private readonly bool errorState;

    public Result(TValue value) {
        this.value = value;
        this.error = null;
        this.errorState = false;
    }

    public Result(Exception error) {
        this.value = default;
        this.error = error;
        this.errorState = true;
    }

    public Result(string errorMessage) {
        this.value = default;
        this.error = new Exception(errorMessage);
        this.errorState = true;
    }

    public bool IsSuccess => !errorState;

    public bool IsError => errorState;

    public Exception? Error => error;

    public TValue Value => TryGetValue_ThrowExceptionOnFail(IsSuccess, value, error);

    private static T TryGetValue_ThrowExceptionOnFail<T>(bool isSuccess, T? value, Exception? error) {
        if (isSuccess) return value!;
        if (error is not null) throw error;
        throw new InvalidOperationException($"Cannot get value when Result is in Error State.");
    }
}
public interface IEnumerableResult<T> : IEnumerable<T>, IResult<IEnumerable<T>> { }
public readonly struct EnumerableResult<TValue> : IEnumerableResult<TValue> {
    private readonly Result<IEnumerable<TValue>> result;

    public EnumerableResult(Result<IEnumerable<TValue>> result) {
        this.result = result;
    }

    public bool IsSuccess => result.IsSuccess;

    public bool IsError => result.IsError;

    public Exception Error => result.Error;

    public IEnumerable<TValue> Value => result.Value;

    public IEnumerator<TValue> GetEnumerator() {
        return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return result.Value.GetEnumerator();
    }
}

public static class Result {
    public static IResult<T> Success<T>(T value) => 
        value is not null ? new Result<T>(value) : new Result<T>(new ArgumentNullException("Value cannot be null on creation of Result"));
    public static IResult<T> Failure<T>(Exception exception) => new Result<T>(exception);
    public static IResult<T> Failure<T>(string message) => new Result<T>(message);
}

public static class ToIResultExtensions {
    public static IResult<T> ToResult<T>(this T value) => Result.Success(value);
    public static async Task<IResult<T>> ToResultAsync<T>(this Task<T> value) => Result.Success(await value);
    public static IEnumerableResult<T> ToResult<T>(this IEnumerable<T> value) => Result.Success(value).AsEnumerable();
    public static async Task<IEnumerableResult<T>> ToResultAsync<T>(this Task<IEnumerable<T>> value) => Result.Success(await value).AsEnumerable();
}

public static class IResultExtensions {
    public static IResult<TOutput> Map<TInput, TOutput>(this IResult<TInput> input, Func<TInput, TOutput> func) =>
        input.IsError ? Result.Failure<TOutput>(input.Error) : Result.Success(func(input.Value));

    public static IEnumerableResult<T> AsEnumerable<T>(this IResult<IEnumerable<T>> input) => (EnumerableResult<T>)input;

    public static IResult<T> Some<T>(this IResult<T> input, Func<T, T> func) =>
        input.IsSuccess ? Result.Success(func(input.Value)) : input;

    public static IResult<T> FixError<T>(this IResult<T> input, Func<T, T> func) =>
        input.IsError ? Result.Success(func(input.Value)) : input;

    public static void Handle<T>(this IResult<T> input, Action<Exception> func) {
        if(input.IsError) func(input.Error);
    }
}

public static class IResultLoggingExtensions {
    public static IResult<T> LogError<T>(this IResult<T> input, ILogger logger, Action<ILogger, Exception> logAction = null) {
        if (input.IsSuccess) {
            return input;
        }
        if (logAction is null) {
            logger.LogError(input.Error, input.Error.Message);
            return input;
        }

        logAction(logger, input.Error);
        return input;
    }

    public static IResult<T> LogWarning<T>(this IResult<T> input, ILogger logger) {
        if (input.IsError)
            logger.LogWarning(input.Error, input.Error.Message);
        return input;
    }

    public static IResult<T> LogInformation<T>(this IResult<T> input, ILogger logger) {
        if (input.IsError)
            logger.LogInformation(input.Error, input.Error.Message);
        return input;
    }
}
