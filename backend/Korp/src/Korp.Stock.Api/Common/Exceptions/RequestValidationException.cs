using FluentValidation.Results;

namespace Korp.Stock.Api.Common.Exceptions
{
    public class RequestValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public RequestValidationException(IEnumerable<ValidationFailure> failures) : base("One or more validation errors occurred.")
        {
            Errors = failures
                .GroupBy(failures => failures.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(failures => failures.ErrorMessage)
                        .ToArray());
        }
    }
}
