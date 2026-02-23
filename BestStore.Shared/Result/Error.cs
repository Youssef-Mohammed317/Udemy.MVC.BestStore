using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Domain.Result
{
    public class Error
    {
        public string Code { get; }
        public string Message { get; }

        private Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public static Error None =>
            new(string.Empty, string.Empty);

        public static Error Failure(string code, string message) =>
            new(code, message);
    }
}
