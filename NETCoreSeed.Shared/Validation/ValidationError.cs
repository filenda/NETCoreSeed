using System;
using System.Collections.Generic;
using System.Text;

namespace NETCoreSeed.Shared.Validation
{
    public class ValidationError
    {
        public string Message { get; set; }
        public ValidationError(string message)
        {
            Message = message;
        }
    }
}