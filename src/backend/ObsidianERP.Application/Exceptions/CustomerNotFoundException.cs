namespace ObsidianERP.Application.Exceptions;

public sealed class CustomerNotFoundException(Guid id)
    : NotFoundException($"Cliente '{id}' não encontrado.");
