namespace JTradeERP.Application.Common.Exceptions;

public sealed class ValidationFailureException : Exception
{
    public ValidationFailureException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
