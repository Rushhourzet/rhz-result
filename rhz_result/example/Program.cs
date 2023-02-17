using Microsoft.Extensions.Logging;
using rhz_result;
using System.Net.Http.Headers;

internal class Program {
    public static void Main(string[] args) {

        #region IResult Instantiation
        //var result1 = new Result<Person>("Harold");       //cannot call constructor from outside the library
        var result2 = Result.Ok("Harold");                  //is valid
        var result3 = Result.Ok<Person>(null);              //result is in error state, because it cannot be created with a null value
        var result4 = Result.Ok(new List<Person>());        //result is valid => can contain empty collections (=> recommended to instantiate Enumerable Collections over EnumerableResult.Ok
        var result5 = Result.Err<Person>();                 //is in error state; Error = new ArgumentNullException("Value cannot be null on creation of Result")
        var result6 = Result.Err<Person>(new Exception());  //result in error state
        var result7 = Result.Err<Person>("message");        //result in error state; Error = new Exception("message")

        #region IResult<IEnumerable> Instantiation
        var r1 = EnumerableResult.Ok(new string[] { "a", "b", "c" }.AsReadOnly());       //valid and recommended to use ReadOnly Collections for instantiation
        var r2 = EnumerableResult.Ok(new string[5]);                        //valid to have an empty collection
        //var r3 = EnumerableResult.Ok<Person>(null);                        //compiler error
        var r4 = EnumerableResult.Ok(Result.Ok("Harold"), Result.Ok("Kumar"), Result.Err<string>("User does not Exist"));                        //compiler error



        var business = Result.Ok(new Business("Har"));
        var fail = Result.Ok<Person>(null);

        //general use
        var harold = business
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