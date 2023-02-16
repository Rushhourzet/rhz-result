using rhz_result;
using rhz_result.Extensions;

namespace rhz_result.Extensions;

public static class IEnumerableResultExtensions
{
    public static IEnumerableResult<T> Where<T>(this IEnumerableResult<T> input, Func<T, bool> func) => EnumerableResult.Success(input.Value.Where(func));
    public static IEnumerableResult<T> Where<T>(this IResult<IEnumerable<T>> input, Func<T, bool> func) => EnumerableResult.Success(input.Value.Where(func));
    public static IEnumerableResult<T> Where<T>(this IResult<IReadOnlyCollection<T>> input, Func<T, bool> func) => EnumerableResult.Success(input.Value.Where(func));

    public static IEnumerableResult<TOutput> Select<TInput, TOutput>(this IEnumerableResult<TInput> input, Func<TInput, TOutput> func) => EnumerableResult.Success(input.Value.Select(func));
    public static IEnumerableResult<TOutput> Select<TInput, TOutput>(this IResult<IEnumerable<TInput>> input, Func<TInput, TOutput> func) => EnumerableResult.Success(input.Value.Select(func));
    public static IEnumerableResult<TOutput> Select<TInput, TOutput>(this IResult<IReadOnlyCollection<TInput>> input, Func<TInput, TOutput> func) => EnumerableResult.Success(input.Value.Select(func));
}
