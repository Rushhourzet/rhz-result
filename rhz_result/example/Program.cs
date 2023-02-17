using Microsoft.Extensions.Logging;
using rhz_result;
using System.Net.Http.Headers;

internal class Program {
    public static void Main(string[] args) {
        var person = Result.Ok(new Business("Har"));
        var fail = Result.Ok<Person>(null);

        //general use
        var harold = person
            .Some(p => new Business(p.name + "old"))
            .Map(p => new Person(p.name))
            .Handle(e => throw e);

        //error correction
        var errorCorrection = Result.Err<Person>("asdf")
            .None(() => new Person("Peter"))
            .Handle(e => throw e);

        //logging errors
        ConsoleLogger logger = new ConsoleLogger();
        var logging = Result
            .Err<Person>(new Exception("I am a real person"))
            .LogError(logger);

        //async tasks
        var task = Task.Run(() => {
                Thread.Sleep(2000);
                return new Person("Sven");
            });
        var taskResult = task
            .ToResultAsync()
            .Then(p => {
                Console.WriteLine(p.name);
                return new Person(null);
            })
            .Handle(e => Console.WriteLine(e.Message))
            .None(() => new Person("Laurel"));

        taskResult.Wait();

        //enumerable Results
        var collection = EnumerableResult
            .Ok(harold!, errorCorrection!, taskResult.Result)
            .Select(p => {
                Console.WriteLine(p.name);
                return p;
            })
            .Aggregate("", (acc, person) => acc + person.name)
            .Some(p => {
                Console.WriteLine(p);
                return p;
            });
    }
}

internal record Person(string name);
internal record Business(string name);

internal class ConsoleLogger : ILogger {
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull =>
        default!;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
        Console.WriteLine($"[{logLevel}]: {exception.Message}]");
    }
}