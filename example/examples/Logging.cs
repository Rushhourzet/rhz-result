using Microsoft.Extensions.Logging;
using rhz.Result;
using rhz.Result.Logging;

namespace example.examples;

internal class Logging {
    internal static void Example(IResult<Person> okResult, IResult<Person> errorResult) {
        ConsoleLogger consoleLogger = new ConsoleLogger();
        //LogError will only log IResult that is in error state and LogOk will only log result that is in Ok state
        var l1 = errorResult.LogError(consoleLogger); //will log: logger.LogError(exception, exception.message);
        var l2 = errorResult.LogError(consoleLogger, (logger, exception) => logger.LogError(exception.Message)); //more control over logging
        var l3 = okResult.LogOk(consoleLogger, (logger, person) => logger.LogTrace(person.name)); //will log

        var l4 = errorResult.LogOk(consoleLogger, (logger, person) => logger.LogTrace(person.name)); //will not execute
        var l5 = okResult.LogError(consoleLogger);
    }
}
