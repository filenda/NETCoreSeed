using System;
using System.Collections.Generic;
using System.Text;

namespace NETCoreSeed.Shared.Validation.Interfaces
{
    public interface IValidation<in TEntity>
    {
        ValidationResult Valid(TEntity entity);
    }
}