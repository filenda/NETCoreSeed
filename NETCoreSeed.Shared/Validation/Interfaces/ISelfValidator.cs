using System;
using System.Collections.Generic;
using System.Text;

namespace NETCoreSeed.Shared.Validation.Interfaces
{
    public interface ISelfValidation
    {
        ValidationResult ValidationResult { get; }
        bool IsValid { get; }
    }
}