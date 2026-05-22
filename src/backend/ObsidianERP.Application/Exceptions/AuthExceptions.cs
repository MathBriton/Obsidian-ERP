namespace ObsidianERP.Application.Exceptions;

public sealed class EmailAlreadyInUseException(string email)
    : Exception($"O e-mail '{email}' já está em uso.");

public sealed class InvalidCredentialsException()
    : Exception("Credenciais inválidas.");

public sealed class InvalidRefreshTokenException()
    : Exception("Refresh token inválido ou expirado.");
