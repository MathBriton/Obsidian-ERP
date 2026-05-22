namespace ObsidianERP.Application.DTOs;

public sealed record CreateCustomerRequest(string Name, string? Email, string? Phone, string? Document);

public sealed record UpdateCustomerRequest(string Name, string? Email, string? Phone, string? Document);

public sealed record CustomerDto(
    Guid Id,
    string Name,
    string? Email,
    string? Phone,
    string? Document,
    DateTime CreatedAt);

public sealed record CustomerQuery(int Page = 1, int PageSize = 10, string? Search = null);
