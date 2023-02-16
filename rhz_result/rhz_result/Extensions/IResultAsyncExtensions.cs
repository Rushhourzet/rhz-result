namespace rhz_result.Extensions;

public static class ToIResultAsyncExtensions {
    public static async Task<IResult<T>> ToResultAsync<T>(this Task<T> value) => Result.Success(await value);
    public static async Task<IEnumerableResult<T>> ToResultAsync<T>(this Task<IEnumerable<T>> value) => EnumerableResult.Success(await value);
}
public static class IResultAsyncExtensions {

    public static async Task<IResult<TOutput>> Then<TInput, TOutput>(this Task<IResult<TInput>> input, Func<TInput, TOutput> func) {
        var inputResult = await input;
        return (IResult<TOutput>)(inputResult.IsSuccess ? Task.FromResult(Result.Success(func(inputResult.Value))) : Task.FromResult(Result.Failure<TOutput>(inputResult.Error)));
    }

    public static async Task<IResult<T>> Handle<T>(this Task<IResult<T>> input, Action<Exception> func) {
        var inputResult = await input;
        if (inputResult.IsError) {
            func(inputResult.Error);
            return Result.Failure<T>(new HasBeenHandledException());
        }
        return inputResult;
    }
}
