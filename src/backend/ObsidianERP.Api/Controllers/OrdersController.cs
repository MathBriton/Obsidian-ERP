using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ObsidianERP.Application.Abstractions;
using ObsidianERP.Application.DTOs;

namespace ObsidianERP.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/orders")]
public sealed class OrdersController(IOrderService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] OrderQuery query, CancellationToken cancellationToken)
        => Ok(await service.GetPagedAsync(query, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await service.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("{id:guid}/items")]
    public async Task<IActionResult> AddItem(Guid id, AddOrderItemRequest request, CancellationToken cancellationToken)
        => Ok(await service.AddItemAsync(id, request, cancellationToken));

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, ChangeOrderStatusRequest request, CancellationToken cancellationToken)
        => Ok(await service.ChangeStatusAsync(id, request, cancellationToken));

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
        => Ok(await service.CancelAsync(id, cancellationToken));
}
