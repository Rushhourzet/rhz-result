using System.Collections;

namespace rhz_result;

public readonly struct EnumerableResult<TValue> : IReadOnlyCollection<TValue> {

    private readonly Result<IReadOnlyCollection<TValue>> result;

    internal EnumerableResult(Result<IReadOnlyCollection<TValue>> result) {
        this.result = result;
    }

    public bool IsOk => result.IsOk;

    public bool IsError => result.IsError;

    public Exception Error => result.Error;

    public IEnumerable<TValue> Value => result.Value;

    public int Count => result.Value.Count;

    public IEnumerator<TValue> GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => result.Value.GetEnumerator();
}


/// <summary>
/// static EnumerableResult class to create EnumerableResults
/// </summary>
public static class EnumerableResult {
    public static IResult<IReadOnlyCollection<T>> Ok<T>(IEnumerable<T> value) =>
        Result.Ok((IReadOnlyCollection<T>)value.ToArray().AsReadOnly());
    public static IResult<IReadOnlyCollection<T>> Ok<T>(IReadOnlyCollection<T> value) => Result.Ok(value);
    public static IResult<IReadOnlyCollection<T>> Ok<T>(params IResult<T>[] inputs) =>
        Result.Ok(
            (IReadOnlyCollection<T>)inputs
            .Where(input => input.IsOk)
            .Select(input => input.Value)
            .ToArray()
            .AsReadOnly());
    
}


public static class IEnumerableResultExtensions {
    public static IResult<IReadOnlyCollection<T>> Where<T>(this IResult<IReadOnlyCollection<T>> input, Func<T, bool> func) => EnumerableResult.Ok(input.Value.Where(func));
    public static IResult<IReadOnlyCollection<T>> Where<T>(this IResult<IEnumerable<T>> input, Func<T, bool> func) => EnumerableResult.Ok(input.Value.Where(func));

    public static IResult<IReadOnlyCollection<TOutput>> Select<TInput, TOutput>(this IResult<IReadOnlyCollection<TInput>> input, Func<TInput, TOutput> func) => 
        EnumerableResult.Ok(input.Value.Select(func));
    public static IResult<IReadOnlyCollection<TOutput>> Select<TInput, TOutput>(this IResult<IEnumerable<TInput>> input, Func<TInput, TOutput> func) => 
        EnumerableResult.Ok(input.Value.Select(func));

    public static IResult<TAccumulate> Aggregate<TInput, TAccumulate>(this IResult<IReadOnlyCollection<TInput>> input, TAccumulate seed, Func<TAccumulate, TInput, TAccumulate> func) => 
        Result.Ok(input.Value.Aggregate(seed,func));
}
