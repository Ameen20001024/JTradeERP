using FluentValidation;
using MediatR;

namespace JTradeERP.Application.Common.Diagnostics;

public sealed record FoundationValidationRequest(string Value) : IRequest<string>;

public sealed class FoundationValidationRequestValidator : AbstractValidator<FoundationValidationRequest>
{
    public FoundationValidationRequestValidator()
    {
        RuleFor(request => request.Value).NotEmpty().MaximumLength(100);
    }
}

public sealed class FoundationValidationRequestHandler : IRequestHandler<FoundationValidationRequest, string>
{
    public Task<string> Handle(FoundationValidationRequest request, CancellationToken cancellationToken)
        => Task.FromResult(request.Value);
}
