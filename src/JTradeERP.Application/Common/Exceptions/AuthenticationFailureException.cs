namespace JTradeERP.Application.Common.Exceptions;
public sealed class AuthenticationFailureException(string message = "Authentication failed.") : Exception(message);
