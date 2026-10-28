using FluentValidation;

namespace JTradeERP.Application.Features.Security.Administration.GetAuditLogs;

public sealed class GetAuditLogsValidator : AbstractValidator<GetAuditLogsQuery>
{
    public GetAuditLogsValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.Page).Must((query, page) => (long)(page - 1) * query.PageSize <= int.MaxValue)
            .WithMessage("The requested page offset is too large.");
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
    }
}
