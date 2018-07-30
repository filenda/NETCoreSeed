using System;
using System.Collections.Generic;
using System.Text;

namespace NETCoreSeed.Shared.Validation
{
    public class ValidationResult
    {
        //public Guid ValidationResultId { get; private set; }
        private readonly List<ValidationError> _erros;
        private string Message { get; set; }
        public bool IsValid { get { return !_erros.Any(); } }
        public IEnumerable<ValidationError> Errors { get { return _erros; } }
        public int IddentityCurrent { get; set; }

        public ValidationResult()
        {
            _erros = new List<ValidationError>();
            //ValidationResultId = Guid.NewGuid();
        }

        public ValidationResult Add(string errorMessage)
        {
            _erros.Add(new ValidationError(errorMessage));
            return this;
        }

        public ValidationResult Add(ValidationError error)
        {
            _erros.Add(error);
            return this;
        }

        public ValidationResult Add(params ValidationResult[] validationResults)
        {
            if (validationResults == null) return this;

            foreach (var result in validationResults.Where(r => r != null))
                _erros.AddRange(result.Errors);

            return this;
        }

        public ValidationResult Remove(ValidationError error)
        {
            if (_erros.Contains(error))
                _erros.Remove(error);
            return this;
        }
    }
}