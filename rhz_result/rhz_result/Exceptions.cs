using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rhz_result {
    public sealed class HasBeenHandledException : Exception {
        private const string MESSAGE = "Error/Exception has already been handled";
        public HasBeenHandledException() : base(MESSAGE) { }
        public HasBeenHandledException(string message) : base(message) { }
    }
    
    public sealed class IEnumerableEmptyException : Exception {
        private const string MESSAGE = "Value of type IEnumerable<T> was empty";
        public IEnumerableEmptyException() : base(MESSAGE) {}

        public IEnumerableEmptyException(string message) : base(message) {}
    }
}
