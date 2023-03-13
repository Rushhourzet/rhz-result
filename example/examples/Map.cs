using rhz_result;

namespace example.examples;

internal class Map {
    internal static void Example(IResult<Person> okResult, IResult<Person> errorResult) {
        var m1 = okResult.Map(person => new Business($"{person.name} Consulting")); //will map to valid IResult<Business>
        var m2 = errorResult.Map(person => new Business($"{person.name} Consulting")); //will map to invalid IResult<Business> and will carry over IResult<Person>.Error
    }
}
