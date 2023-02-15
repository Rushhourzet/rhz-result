using Microsoft.Extensions.Logging;
using System.Collections;

namespace rhz_result;

/// <summary>
/// object can go into Error State
/// </summary>
public interface IErrorable {

    /// <summary>
    /// object is not in Error State
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// object is in Error State
    /// </summary>
    public bool IsError { get; }

    /// <summary>
    /// the error the object can return
    /// </summary>
    public Exception Error { get; }
}

/// <summary>
/// generic Value wrapper
/// </summary>
/// <typeparam name="T">Type of Value</typeparam>
public interface IValue<T> {
    /// <summary>
    /// gets the wrapped Value
    /// </summary>
    public T Value { get; }
}

/// <summary>
/// Result wrapper interface that either holds a Value or an Error, depending on Error State
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IResult<T> : IErrorable, IValue<T> { }
public readonly struct Result<TValue> : IResult<TValue> {
    private readonly TValue? value;
    private readonly Exception? error;
    private readonly bool errorState;

    internal Result(TValue value) {
        this.value = value;
        this.error = null;
        this.errorState = false;
    }

    internal Result(Exception error) {
        this.value = default;
        this.error = error;
        this.errorState = true;
    }

    internal Result(string errorMessage) {
        this.value = default;
        this.error = new Exception(errorMessage);
        this.errorState = true;
    }

    public bool IsSuccess => !errorState;
    public bool IsError => errorState;

    public Exception Error => IsError ? error! : throw new InvalidOperationException("Cannot get Error when Result is not in Error State");

    public TValue Value => TryGetValue_ThrowExceptionOnFail(IsSuccess, value, error);

    private static T TryGetValue_ThrowExceptionOnFail<T>(bool isSuccess, T? value, Exception? error) {
        if (isSuccess) return value!;
        if (error is not null) throw error;
        throw new InvalidOperationException($"Cannot get Value when Result is in Error State.");
    }
}

/// <summary>
/// Represents an IResult of ReadonlyCollection of T and is a QoL wrapper to directly enumerate on IEnumerableResult without having to call result.Value
/// </summary>
/// <typeparam name="T">Type of Item in IReadonlyCollection</typeparam>
public interface IEnumerableResult<T> : IReadOnlyCollection<T>, IResult<IReadOnlyCollection<T>> { }
public readonly struct EnumerableResult<TValue> : IReadOnlyCollection<TValue> {
    private readonly Result<IReadOnlyCollection<TValue>> result;

    internal EnumerableResult(Result<IReadOnlyCollection<TValue>> result) {
        this.result = result;
    }

    public bool IsSuccess => result.IsSuccess;

    public bool IsError => result.IsError;

    public Exception Error => result.Error;

    public IEnumerable<TValue> Value => result.Value;

    public int Count => result.Value.Count;

    public IEnumerator<TValue> GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => result.Value.GetEnumerator();
}

/// <summary>
/// static Result class to create Results
/// </summary>
public static class Result {

    /// <summary>
    /// Creates a new IResult of T. If value is null, it will create an Error State Result
    /// </summary>
    /// <typeparam name="T">Type of Value</typeparam>
    /// <param name="value">Value of Type T</param>
    /// <returns>Result of T</returns>
    public static IResult<T> Success<T>(T value) =>
        value is not null ? new Result<T>(value) : new Result<T>(new ArgumentNullException("Value cannot be null on creation of Result"));

    /// <summary>
    /// Creates a new Error State IResult of T. If Exception is null it will default to new Exception() as Error
    /// </summary>
    /// <typeparam name="T">Type of Value</typeparam>
    /// <param name="value">Value of Type T</param>
    /// <returns>Result of T</returns>
    public static IResult<T> Failure<T>(Exception exception) =>
        exception is not null ? new Result<T>(exception) : new Result<T>(new Exception());


    /// <summary>
    /// Creates a new Error State IResult of T. If Exception is null it will default to new Exception() as Error
    /// </summary>
    /// <typeparam name="T">Type of Value</typeparam>
    /// <param name="value">Value of Type T</param>
    /// <returns>Result of T</returns>
    public static IResult<T> Failure<T>() => new Result<T>(new Exception());


    /// <summary>
    /// Creates a new Error State IResult of T. If Exception is null it will default to new Exception() as Error
    /// </summary>
    /// <typeparam name="T">Type of Value</typeparam>
    /// <param name="value">Value of Type T</param>
    /// <returns>Result of T</returns>
    public static IResult<T> Failure<T>(string message) => new Result<T>(message);
}

/// <summary>
/// static EnumerableResult class to create EnumerableResults
/// </summary>
public static class EnumerableResult {
    public static IEnumerableResult<T> Success<T>(IEnumerable<T> value) => (IEnumerableResult<T>)Result.Success((IReadOnlyCollection<T>)value.ToList().AsReadOnly());
    public static IEnumerableResult<T> Success<T>(IReadOnlyCollection<T> value) => (IEnumerableResult<T>)Result.Success(value);
}

public static class ToIResultExtensions {
    public static IResult<T> ToResult<T>(this T value) => Result.Success(value);
    public static async Task<IResult<T>> ToResultAsync<T>(this Task<T> value) => Result.Success(await value);
    public static IEnumerableResult<T> ToResult<T>(this IEnumerable<T> value) => EnumerableResult.Success(value);
    public static async Task<IEnumerableResult<T>> ToResultAsync<T>(this Task<IEnumerable<T>> value) => EnumerableResult.Success(await value);
}

public static class IResultExtensions {
    public static IResult<TOutput> Map<TInput, TOutput>(this IResult<TInput> input, Func<TInput, TOutput> func) =>
        input.IsError ? Result.Failure<TOutput>(input.Error) : Result.Success(func(input.Value));

    public static IEnumerableResult<T> AsEnumerable<T>(this IResult<IEnumerable<T>> input) => EnumerableResult.Success(input.Value);
    public static IEnumerableResult<T> AsEnumerable<T>(this IResult<IReadOnlyCollection<T>> input) => EnumerableResult.Success(input.Value);

    public static IResult<T> Some<T>(this IResult<T> input, Func<T, T> func) =>
        input.IsSuccess ? Result.Success(func(input.Value)) : input;

    public static IResult<T> FixError<T>(this IResult<T> input, Func<T, T> func) =>
        input.IsError ? Result.Success(func(input.Value)) : input;

    public static void Handle<T>(this IResult<T> input, Action<Exception> func) {
        if (input.IsError) func(input.Error);
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

public static class IEnumerableResultExtensions {
    public static IEnumerableResult<T> Where<T>(this IEnumerableResult<T> input, Func<T, bool> func) => EnumerableResult.Success(input.Value.Where(func));
    public static IEnumerableResult<T> Where<T>(this IResult<IEnumerable<T>> input, Func<T, bool> func) => EnumerableResult.Success(input.Value.Where(func));
    public static IEnumerableResult<T> Where<T>(this IResult<IReadOnlyCollection<T>> input, Func<T, bool> func) => EnumerableResult.Success(input.Value.Where(func));

    public static IEnumerableResult<TOutput> Select<TInput, TOutput>(this IEnumerableResult<TInput> input, Func<TInput, TOutput> func) => EnumerableResult.Success(input.Value.Select(func));
    public static IEnumerableResult<TOutput> Select<TInput, TOutput>(this IResult<IEnumerable<TInput>> input, Func<TInput, TOutput> func) => EnumerableResult.Success(input.Value.Select(func));
    public static IEnumerableResult<TOutput> Select<TInput, TOutput>(this IResult<IReadOnlyCollection<TInput>> input, Func<TInput, TOutput> func) => EnumerableResult.Success(input.Value.Select(func));
}
