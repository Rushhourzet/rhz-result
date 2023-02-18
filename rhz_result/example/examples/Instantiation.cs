using rhz_result;

namespace example.examples;
internal static class Instantiation {
    internal static void Example() {
        //var result1 = new Result<Person>("Harold");       //cannot call constructor from outside the library
        var result2 = Result.Ok("Harold");                  //is valid
        var result3 = Result.Ok<Person>(null);              //result is in error state, because it cannot be created with a null value
        var result4 = Result.Ok(new List<Person>());        //result is valid => can contain empty collections (=> recommended to instantiate Enumerable Collections over EnumerableResult.Ok
        var result5 = Result.Err<Person>();                 //is in error state; Error = new ArgumentNullException("Value cannot be null on creation of Result")
        var result6 = Result.Err<Person>(new Exception());  //result in error state
        var result7 = Result.Err<Person>("message");        //result in error state; Error = new Exception("message")
    }
}
