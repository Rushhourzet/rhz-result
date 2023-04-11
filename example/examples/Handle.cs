using rhz.Result;

namespace example.examples;

internal class Handle {
    internal static void Example(IResult<Person> okResult, IResult<Person> errorResult) {
        var h1 = okResult.Handle(e => Console.WriteLine(e)); //will not execute
        var h2 = errorResult.Handle(e => Console.WriteLine(e)); //will execute
    }
}