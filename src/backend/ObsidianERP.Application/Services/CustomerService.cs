using ObsidianERP.Application.Abstractions;
using ObsidianERP.Application.Common;
using ObsidianERP.Application.DTOs;
using ObsidianERP.Application.Exceptions;
using ObsidianERP.Domain.Entities;

namespace ObsidianERP.Application.Services;

public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository) => _repository = repository;

    public async Task<PagedResult<CustomerDto>> GetPagedAsync(
        CustomerQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is < 1 or > 100 ? 10 : query.PageSize;

        var (items, total) = await _repository.GetPagedAsync(page, pageSize, query.Search, cancellationToken);

        return new PagedResult<CustomerDto>(items.Select(Map).ToList(), page, pageSize, total);
    }

    public async Task<CustomerDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new CustomerNotFoundException(id);

        return Map(customer);
    }

    public async Task<CustomerDto> CreateAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var customer = new Customer
        {
            Name = request.Name.Trim(),
            Email = Normalize(request.Email),
            Phone = request.Phone?.Trim(),
            Document = request.Document?.Trim(),
        };

        await _repository.AddAsync(customer, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(customer);
    }

    public async Task<CustomerDto> UpdateAsync(
        Guid id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var customer = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new CustomerNotFoundException(id);

        customer.Name = request.Name.Trim();
        customer.Email = Normalize(request.Email);
        customer.Phone = request.Phone?.Trim();
        customer.Document = request.Document?.Trim();
        customer.UpdatedAt = DateTime.UtcNow;

        _repository.Update(customer);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(customer);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new CustomerNotFoundException(id);

        customer.IsDeleted = true;
        customer.DeletedAt = DateTime.UtcNow;

        _repository.Update(customer);
        await _repository.SaveChangesAsync(cancellationToken);
    }

    private static string? Normalize(string? email) =>
        string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLowerInvariant();

    private static CustomerDto Map(Customer customer) =>
        new(customer.Id, customer.Name, customer.Email, customer.Phone, customer.Document, customer.CreatedAt);
}
