using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rhz.Result; 
public sealed class ResultHasBeenHandledException : Exception {
    private const string MESSAGE = "Error/Exception has already been handled.";
    public ResultHasBeenHandledException() : base(MESSAGE) { }
    public ResultHasBeenHandledException(string message) : base(message) { }
}

public sealed class ResultValidationException<T> : Exception {
    private const string MESSAGE = "Result is invalid.";
    private readonly T value;

    public override string Message => $"[{value}]: {base.Message}"; 
    public ResultValidationException(T value, string message) : base(message){ 
        this.value = value;
    }
    public ResultValidationException(T value) : base(MESSAGE){ 
        this.value = value;
    }
}

public sealed class IEnumerableEmptyException : Exception {
    private const string MESSAGE = "Value of type IEnumerable<T> was empty.";
    public IEnumerableEmptyException() : base(MESSAGE) {}

    public IEnumerableEmptyException(string message) : base(message) {}
}
