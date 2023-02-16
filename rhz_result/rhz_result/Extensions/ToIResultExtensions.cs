namespace rhz_result.Extensions;

public static class ToIResultExtensions {
    public static IResult<T> ToResult<T>(this T value) => Result.Success(value);
    public static IEnumerableResult<T> ToResult<T>(this IEnumerable<T> value) => EnumerableResult.Success(value);
}
