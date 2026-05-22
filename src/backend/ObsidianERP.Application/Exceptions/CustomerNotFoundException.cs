namespace ObsidianERP.Application.Exceptions;

public sealed class CustomerNotFoundException(Guid id)
    : Exception($"Cliente '{id}' não encontrado.");
