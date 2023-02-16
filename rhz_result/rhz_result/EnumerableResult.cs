using System.Collections;

namespace rhz_result;

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
/// static EnumerableResult class to create EnumerableResults
/// </summary>
public static class EnumerableResult {
    public static IEnumerableResult<T> Success<T>(IEnumerable<T> value) => (IEnumerableResult<T>)Result.Success((IReadOnlyCollection<T>)value.ToList().AsReadOnly());
    public static IEnumerableResult<T> Success<T>(IReadOnlyCollection<T> value) => (IEnumerableResult<T>)Result.Success(value);
}


public static class IEnumerableResultExtensions {
    public static IEnumerableResult<T> Where<T>(this IEnumerableResult<T> input, Func<T, bool> func) => EnumerableResult.Success(input.Value.Where(func));
    public static IEnumerableResult<T> Where<T>(this IResult<IEnumerable<T>> input, Func<T, bool> func) => EnumerableResult.Success(input.Value.Where(func));
    public static IEnumerableResult<T> Where<T>(this IResult<IReadOnlyCollection<T>> input, Func<T, bool> func) => EnumerableResult.Success(input.Value.Where(func));

    public static IEnumerableResult<TOutput> Select<TInput, TOutput>(this IEnumerableResult<TInput> input, Func<TInput, TOutput> func) => EnumerableResult.Success(input.Value.Select(func));
    public static IEnumerableResult<TOutput> Select<TInput, TOutput>(this IResult<IEnumerable<TInput>> input, Func<TInput, TOutput> func) => EnumerableResult.Success(input.Value.Select(func));
    public static IEnumerableResult<TOutput> Select<TInput, TOutput>(this IResult<IReadOnlyCollection<TInput>> input, Func<TInput, TOutput> func) => EnumerableResult.Success(input.Value.Select(func));
}
