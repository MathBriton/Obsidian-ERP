namespace ObsidianERP.Application.Exceptions;

/// <summary>
/// Base para exceções que representam um recurso inexistente (mapeadas para HTTP 404).
/// </summary>
public abstract class NotFoundException(string message) : Exception(message);
