namespace ObsidianERP.Application.Exceptions;

public sealed class OrderNotFoundException(Guid id)
    : Exception($"Pedido '{id}' não encontrado.");
