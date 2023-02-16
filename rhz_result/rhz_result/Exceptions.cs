using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rhz_result {
    public sealed class HasBeenHandledException : Exception {
        private const string MESSAGE = "Error/Exception has already been handled";
        public override string Message => MESSAGE;
        public HasBeenHandledException() : base(MESSAGE) {}
    }
}
