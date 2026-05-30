namespace ObsidianERP.Application.Exceptions;

public sealed class OrderNotFoundException(Guid id)
    : NotFoundException($"Pedido '{id}' não encontrado.");
