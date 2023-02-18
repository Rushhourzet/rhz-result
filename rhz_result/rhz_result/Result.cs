using System;
using System.Collections;

namespace rhz_result;

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

    public bool IsOk => !errorState;
    public bool IsError => errorState;

    public Exception Error => IsError ? error! : throw new InvalidOperationException("Cannot get Error when Result is not in Error State");

    public TValue Value => TryGetValue_ThrowExceptionOnFail(IsOk, value, error);

    private static T TryGetValue_ThrowExceptionOnFail<T>(bool isSuccess, T? value, Exception? error) {
        if (isSuccess) return value!;
        if (error is not null) throw error;
        throw new InvalidOperationException($"Cannot get Value when Result is in Error State.");
    }
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
    public static IResult<T> Ok<T>(T value) =>
        value is not null ? new Result<T>(value) : new Result<T>(new ArgumentNullException("Value cannot be null on creation of Result"));

    /// <summary>
    /// Creates a new Error State IResult of T. If Exception is null it will default to new Exception() as Error
    /// </summary>
    /// <typeparam name="T">Type of Value</typeparam>
    /// <param name="value">Value of Type T</param>
    /// <returns>Result of T</returns>
    public static IResult<T> Err<T>(Exception exception) =>
        exception is not null ? new Result<T>(exception) : new Result<T>(new Exception());

    public static IResult<T> Err<T>(IResult<T> input) => Err<T>(input.Error);

    /// <summary>
    /// Creates a new Error State IResult of T. If Exception is null it will default to new Exception() as Error
    /// </summary>
    /// <typeparam name="T">Type of Value</typeparam>
    /// <param name="value">Value of Type T</param>
    /// <returns>Result of T</returns>
    public static IResult<T> Err<T>() => new Result<T>(new Exception());


    /// <summary>
    /// Creates a new Error State IResult of T. If Exception is null it will default to new Exception() as Error
    /// </summary>
    /// <typeparam name="T">Type of Value</typeparam>
    /// <param name="value">Value of Type T</param>
    /// <returns>Result of T</returns>
    public static IResult<T> Err<T>(string message) => new Result<T>(message);
}

public static class IResultExtensions {

    public static IResult<TOutput> Map<TInput, TOutput>(this IResult<TInput> input, Func<TInput, TOutput> fn) =>
        input.IsError ? Result.Err<TOutput>(input.Error) : Result.Ok(fn(input.Value));

    public static IResult<IEnumerable<T>> AsEnumerable<T>(this IResult<IEnumerable<T>> input) => EnumerableResult.Ok(input.Value);

    public static IResult<T> Some<T>(this IResult<T> input, Func<T, IResult<T>> fn) =>
        input.IsOk ? fn(input.Value) : input;

    public static IResult<T> Some<T>(this IResult<T> input, Func<T, T> fn) =>
        Some(input, value => Result.Ok(fn(value)));


    public static IResult<T> None<T>(this IResult<T> input, Func<Exception, IResult<T>> fn) {
        if (input.IsError) {
            return fn(input.Error);
        }
        return input;
    }

    public static IResult<T> None<T>(this IResult<T> input, Func<IResult<T>> fn) {
        if(input.IsError) {
            return fn();
        }
        return input;
    }

    public static IResult<T> None<T>(this IResult<T> input, Func<Exception, T> fn) => None(input, e => Result.Ok(fn(e)));
    public static IResult<T> None<T>(this IResult<T> input, Func<Exception, Exception> fn) => None(input, e => Result.Err<T>(fn(e)));

    public static IResult<T> Handle<T>(this IResult<T> input, Action<Exception> fn) {
        if (input.IsError) {
            fn(input.Error);
            return Result.Err<T>(new ResultHasBeenHandledException());
        }
        return input;
    }

    public static IResult<T> Validate<T>(this IResult<T> input, Predicate<T> condition) =>
        input.Some(value => condition(value) ? Result.Ok(value) : Result.Err<T>(new ResultValidationException<T>(value)));

    public static bool Deconstruct<T>(this IResult<T> input, out T? value, out Exception? error) {
        Deconstruct(input, out bool isOk, out value, out error);
        return isOk;
    }
    public static void Deconstruct<T>(this IResult<T> input, out bool isOk, out T? value, out Exception? error) {
        isOk = input.IsOk;
        value = input.IsOk ? input.Value : default;
        error = input.IsError ? input.Error : null;
    }
}