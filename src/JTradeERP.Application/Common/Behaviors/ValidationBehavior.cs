using FluentValidation;
using MediatR;
using JTradeERP.Application.Common.Exceptions;

namespace JTradeERP.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validatorsArray = validators.ToArray();
        if (validatorsArray.Length == 0)
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(validatorsArray.Select(v => v.ValidateAsync(context, cancellationToken)));
        var errors = results
            .SelectMany(result => result.Errors)
            .Where(error => error is not null)
            .GroupBy(error => error.PropertyName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).Distinct().ToArray(), StringComparer.OrdinalIgnoreCase);

        if (errors.Count > 0)
        {
            throw new ValidationFailureException(errors);
        }

        return await next();
    }
}
