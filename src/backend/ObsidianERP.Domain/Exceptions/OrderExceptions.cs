namespace ObsidianERP.Domain.Exceptions;

public abstract class DomainException(string message) : Exception(message);

public sealed class EmptyOrderException()
    : DomainException("Um pedido não pode ficar vazio.");

public sealed class OrderCannotBeModifiedException()
    : DomainException("Um pedido cancelado não pode ser modificado.");

public sealed class OrderAlreadyCancelledException()
    : DomainException("O pedido já está cancelado.");
