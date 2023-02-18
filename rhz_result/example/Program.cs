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
        #endregion


        #region IResult<IEnumerable> Instantiation
        var r1 = EnumerableResult.Ok(new string[] { "a", "b", "c" }.AsReadOnly());                                              //valid and recommended to use ReadOnly Collections for instantiation
        var r2 = Result.Ok((IEnumerable<string>)new string[] { "a", "b", "c" });                                                //also valid
        var r3 = Result.Ok((IEnumerable<string>)new string[] { "a", "b", "c" }.AsReadOnly());                                   //also valid, but requires case to use ReadOnlyCollection or everything
        var r5 = EnumerableResult.Ok(new string[5]);                                                                            //valid to have an empty collection
        //var r6 = EnumerableResult.Ok<Person>(null);                                                                           //compiler error
        var r7 = EnumerableResult.Ok(Result.Ok("Harold"), Result.Ok("Kumar"), Result.Err<string>("Name does not Exist"));       //valid instantiation
        //var r8 = EnumerableResult.Err();                                                                                      //No EnumerableResult.Err method
        var r9 = Result.Err<IEnumerable<Person>>();                                                                             //Use Result.Err instead => reduces duplicate code in library
        var r10 = Result.Ok(new string[] { "a", "b", "c" });                                                                    //valid, but not recommended, because you cannot use IEnumerableResult extensions on that Result
        #endregion


        #region Some and None
        static IResult<Person> Foo(){
            return Result.Ok(new Person("Person from database, can be either Err or Ok"))       //We instantiate a valid or invalid result (in this case valid)
                .Some(person => new Person("Harold from Database"))                             //this will execute when result is valid
                .None(exception => new Person("default"));                                      //this will execute when result is invalid
        }

        static IResult<Person> FooNone() {
            return Result.Ok<Person>(null)                                                                      //invalid result (maybe a failed db call)
                .Some(p => new Person($"{p.name} from Database"))                                               //this will not execute lambda
                .None(e => Result.Err<Person>(new CustomException("Could not fetch person from database")));    //this will execute lambda
        }

        static IResult<Person> FooSome() {
            return Result.Ok(new Person("Harold"))                                                              //valid result
                .Some(p => new Person($"{p.name} from Database"))                                               //this will execute
                .None(e => Result.Err<Person>(new CustomException("Could not fetch person from database")));    //this will not execute
        }
        #endregion

        var okResult = FooSome();
        var errorResult = FooNone();

        #region Map
        var m1 = okResult.Map(person => new Business($"{person.name} Consulting"));  //will map to valid IResult<Business>

        var m2 = errorResult.Map(person => new Business($"{person.name} Consulting"));  //will map to invalid IResult<Business> and will carry over IResult<Person>.Error
        #endregion


        #region Handle and Logging
        ConsoleLogger consoleLogger = new ConsoleLogger();
        var h1 = okResult.Handle(e => Console.WriteLine(e));     //will not execute
        var h2 = errorResult.Handle(e => Console.WriteLine(e));  //will execute

        //LogError will only log IResult that is in error state and LogOk will only log result that is in Ok state
        var l1 = errorResult.LogError(consoleLogger);                                                            //will log: logger.LogError(exception, exception.message);
        var l2 = errorResult.LogError(consoleLogger, (logger, exception) => logger.LogError(exception.Message)); //more control over logging
        var l3 = okResult.LogOk(consoleLogger, (logger, person) => logger.LogTrace(person.name));
        #endregion


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
        Console.WriteLine($"[{logLevel}]: {exception.Message}]");
    }
}