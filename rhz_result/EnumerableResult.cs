using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace rhz_result;

public readonly struct EnumerableResult<TValue> : IEnumerable<TValue> {

    private readonly Result<IEnumerable<TValue>> result;

    internal EnumerableResult(Result<IEnumerable<TValue>> result) {
        this.result = result;        
    }

    public bool IsOk => result.IsOk;

    public bool IsError => result.IsError;

    public Exception Error => result.Error;

    public IEnumerable<TValue> Value => result.Value;

    public IEnumerator<TValue> GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => result.Value.GetEnumerator();
}


/// <summary>
/// static EnumerableResult class to create EnumerableResults
/// </summary>
public static class EnumerableResult {
    public static IResult<IEnumerable<T>> Ok<T>(IEnumerable<T> value) => Result.Ok(value);
    public static IResult<IEnumerable<T>> Ok<T>(params IResult<T>[] inputs) =>
        Result.Ok(
            (IEnumerable<T>)inputs
            .Where(input => input.IsOk)
            .Select(input => input.Value)
            .ToArray()
            .AsReadOnly());    
}


public static class IEnumerableResultExtensions {
    public static IResult<IEnumerable<T>> Where<T>(this IResult<IEnumerable<T>> input, Func<T, bool> func) => EnumerableResult.Ok(input.Value.Where(func));

    public static IResult<IEnumerable<TOutput>> Select<TInput, TOutput>(this IResult<IEnumerable<TInput>> input, Func<TInput, TOutput> func) => 
        EnumerableResult.Ok(input.Value.Select(func));

    public static IResult<TAccumulate> Aggregate<TInput, TAccumulate>(this IResult<IEnumerable<TInput>> input, TAccumulate seed, Func<TAccumulate, TInput, TAccumulate> func) => 
        Result.Ok(input.Value.Aggregate(seed,func));

    public static IResult<IEnumerable<T>> Any<T>(this IResult<IEnumerable<T>> input) =>
        input.Value.Any() ? Result.Ok(input.Value) : Result.Err<IEnumerable<T>>(new IEnumerableEmptyException($"IEnumerable<{typeof(T)}> was empty"));
}
