﻿using rhz.Result;

namespace example.examples;

internal class Validate {
    internal static void Example(IResult<Person> okResult, IResult<Person> errorResult) {
        var v1 = Result.Ok("Harold")
            .Validate(name => name == "Harold") //will execute and return a valid result with "Harold"
            .Validate(name => name == "Steven"); //maps to an invalid result containing new ResultValidationException<T>(person)
        var v2 = errorResult.Validate(_ => false); //will not execute, because the result is already invalid
    }
}