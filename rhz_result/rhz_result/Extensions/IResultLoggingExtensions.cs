using Microsoft.Extensions.Logging;

namespace rhz_result;

public static class IResultLoggingExtensions {
    public static IResult<T> LogError<T>(this IResult<T> input, ILogger logger, Action<ILogger, Exception> logAction = null) {
        if (input.IsOk) {
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
