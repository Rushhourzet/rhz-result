using example.examples;
using Microsoft.Extensions.Logging;
using rhz.Result;

internal class Program {
    internal static readonly IResult<Person> okResult = Result.Ok(new Person("Harold"));
    internal static readonly IResult<Person> errorResult = Result.Err<Person>("error");
    public static void Main(string[] args) {

        Instantiation.Example();

        IResultIEnumerableInstantiation.Example();

        SomeAndNone.Example();

        Map.Example(okResult, errorResult);

        Validate.Example(okResult, errorResult);

        Handle.Example(okResult, errorResult);

        Logging.Example(okResult, errorResult);


        #region Filter, Map, Reduce of IResult<IEnumerable<T>>

        #endregion

        ////async tasks
        //var task = Task.Run(() => {
        //        Thread.Sleep(2000);
        //        return new Person("Sven");
        //    });
        //var taskResult = task
        //    .ToResultAsync()
        //    .Then(p => {
        //        Console.WriteLine(p.name);
        //        return new Person(null);
        //    })
        //    .Handle(e => Console.WriteLine(e.Message))
        //    .None(() => new Person("Laurel"));

        //taskResult.Wait();

        ////enumerable Results
        //var collection = EnumerableResult
        //    .Ok(harold!, errorCorrection!, taskResult.Result)
        //    .Select(p => {
        //        Console.WriteLine(p.name);
        //        return p;
        //    })
        //    .Aggregate("", (acc, person) => acc + person.name)
        //    .Some(p => {
        //        Console.WriteLine(p);
        //        return p;
        //    });
    }
}

internal class CustomException : Exception {
    public CustomException(string? message) : base(message) {
    }
}
internal record Person(string name);
internal record Business(string name);

internal class ConsoleLogger : ILogger {
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull =>
        default!;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
        Console.WriteLine($"[{logLevel}]: {exception?.Message}]");
    }
}