using ObsidianERP.Application.Common;
using ObsidianERP.Application.DTOs;

namespace ObsidianERP.Application.Abstractions;

public interface ICustomerService
{
    Task<PagedResult<CustomerDto>> GetPagedAsync(CustomerQuery query, CancellationToken cancellationToken = default);
    Task<CustomerDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<CustomerDto> UpdateAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
