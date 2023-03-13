namespace rhz_result;

public static class ToIResultExtensions {
    public static IResult<T> ToResult<T>(this T value) => Result.Ok(value);
    public static IResult<IEnumerable<T>> ToResult<T>(this IEnumerable<T> value) => EnumerableResult.Ok(value);
}
