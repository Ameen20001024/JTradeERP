using JTradeERP.Application.Common.Persistence; using JTradeERP.Application.Common.Security; using JTradeERP.Domain.Security; using MediatR;
namespace JTradeERP.Application.Features.Security.Logout;
public sealed class LogoutHandler(IApplicationDbContext db,ITokenService tokens,IClock clock) : IRequestHandler<LogoutCommand>
{ public async Task Handle(LogoutCommand r,CancellationToken ct){if(!string.IsNullOrWhiteSpace(r.RefreshToken)){var hash=tokens.HashOpaqueToken(r.RefreshToken);var token=db.RefreshTokens.SingleOrDefault(x=>x.TokenHash==hash);if(token is not null&&token.RevokedAt is null){token.Revoke(clock.UtcNow,"logout");db.Add(new SecurityAuditEntry(r.UserId,"Logout","RefreshToken",token.Id.ToString(),"active","revoked",Guid.NewGuid().ToString()));await db.SaveChangesAsync(ct);}}} }
