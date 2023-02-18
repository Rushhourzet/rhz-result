using rhz_result;

namespace example.examples;
internal static class IResultIEnumerableInstantiation {
    internal static void Example() {
        var r1 = EnumerableResult.Ok(new string[] { "a", "b", "c" }.AsReadOnly());                                              //valid and recommended to use ReadOnly Collections for instantiation
        var r2 = Result.Ok((IEnumerable<string>)new string[] { "a", "b", "c" });                                                //also valid
        var r3 = Result.Ok((IEnumerable<string>)new string[] { "a", "b", "c" }.AsReadOnly());                                   //also valid, but requires case to use ReadOnlyCollection or everything
        var r5 = EnumerableResult.Ok(new string[5]);                                                                            //valid to have an empty collection
        //var r6 = EnumerableResult.Ok<Person>(null);                                                                           //compiler error
        var r7 = EnumerableResult.Ok(Result.Ok("Harold"), Result.Ok("Kumar"), Result.Err<string>("Name does not Exist"));       //valid instantiation
        //var r8 = EnumerableResult.Err();                                                                                      //No EnumerableResult.Err method
        var r9 = Result.Err<IEnumerable<Person>>();                                                                             //Use Result.Err instead => reduces duplicate code in library
        var r10 = Result.Ok(new string[] { "a", "b", "c" });
    }
}
