using System;
using System.Collections.Generic;
using System.Text;

namespace NETCoreSeed.Shared.Validation.Interfaces
{
    public interface IValidationRule<in TEntity>
    {
        string ErrorMessage { get; }
        bool Valid(TEntity entity);
    }
}