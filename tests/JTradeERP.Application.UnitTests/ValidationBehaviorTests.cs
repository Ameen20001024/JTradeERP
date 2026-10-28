using FluentValidation;
using MediatR;
using JTradeERP.Application.Common.Behaviors;
using JTradeERP.Application.Common.Diagnostics;
using JTradeERP.Application.Common.Exceptions;

namespace JTradeERP.Application.UnitTests;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task ValidationBehavior_WithValidRequest_CallsNext()
    {
        var behavior = new ValidationBehavior<FoundationValidationRequest, string>([
            new FoundationValidationRequestValidator()
        ]);
        var called = false;

        var result = await behavior.Handle(
            new FoundationValidationRequest("valid"),
            () =>
            {
                called = true;
                return Task.FromResult("handled");
            },
            CancellationToken.None);

        Assert.True(called);
        Assert.Equal("handled", result);
    }

    [Fact]
    public async Task ValidationBehavior_WithInvalidRequest_ThrowsValidationFailure()
    {
        var behavior = new ValidationBehavior<FoundationValidationRequest, string>([
            new FoundationValidationRequestValidator()
        ]);
        var called = false;

        await Assert.ThrowsAsync<ValidationFailureException>(() => behavior.Handle(
            new FoundationValidationRequest(string.Empty),
            () =>
            {
                called = true;
                return Task.FromResult("should-not-run");
            },
            CancellationToken.None));

        Assert.False(called);
    }

    [Fact]
    public async Task ValidationBehavior_WithNoValidators_CallsNext()
    {
        var behavior = new ValidationBehavior<FoundationValidationRequest, string>(Enumerable.Empty<IValidator<FoundationValidationRequest>>());
        var result = await behavior.Handle(
            new FoundationValidationRequest("valid"),
            () => Task.FromResult("handled"),
            CancellationToken.None);

        Assert.Equal("handled", result);
    }
}
