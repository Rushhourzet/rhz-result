using rhz.Result;

namespace example.examples;
internal class SomeAndNone {
    internal static void Example() {
        static IResult<Person> Foo() {
            return Result.Ok(new Person("Person from database, can be either Err or Ok")) //We instantiate a valid or invalid result (in this case valid)
                .Some(person => new Person("Harold from Database")) //this will execute when result is valid
                .None(exception => new Person("default")); //this will execute when result is invalid
        }

        static IResult<Person> FooNone() {
            return Result.Ok<Person>(null) //invalid result (maybe a failed db call)
                .Some(p => new Person($"{p.name} from Database")) //this will not execute lambda
                .None(e => Result.Err<Person>(new CustomException("Could not fetch person from database"))); //this will execute lambda
        }

        static IResult<Person> FooSome() {
            return Result.Ok(new Person("Harold")) //valid result
                .Some(p => new Person($"{p.name} from Database")) //this will execute
                .None(e => Result.Err<Person>(new CustomException("Could not fetch person from database"))); //this will not execute
        }
    }
}
