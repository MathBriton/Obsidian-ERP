using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ObsidianERP.Application.Abstractions;
using ObsidianERP.Application.DTOs;
using ObsidianERP.Application.Exceptions;
using ObsidianERP.Domain.Exceptions;

namespace ObsidianERP.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderService _service;
    private readonly IValidator<CreateOrderRequest> _createValidator;
    private readonly IValidator<AddOrderItemRequest> _addItemValidator;

    public OrdersController(
        IOrderService service,
        IValidator<CreateOrderRequest> createValidator,
        IValidator<AddOrderItemRequest> addItemValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _addItemValidator = addItemValidator;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] OrderQuery query, CancellationToken cancellationToken)
    {
        return Ok(await _service.GetPagedAsync(query, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _service.GetByIdAsync(id, cancellationToken));
        }
        catch (OrderNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(ToModelState(validation));
        }

        try
        {
            var created = await _service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (CustomerNotFoundException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (EmptyOrderException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/items")]
    public async Task<IActionResult> AddItem(Guid id, AddOrderItemRequest request, CancellationToken cancellationToken)
    {
        var validation = await _addItemValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(ToModelState(validation));
        }

        try
        {
            return Ok(await _service.AddItemAsync(id, request, cancellationToken));
        }
        catch (OrderNotFoundException)
        {
            return NotFound();
        }
        catch (DomainException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, ChangeOrderStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _service.ChangeStatusAsync(id, request, cancellationToken));
        }
        catch (OrderNotFoundException)
        {
            return NotFound();
        }
        catch (DomainException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _service.CancelAsync(id, cancellationToken));
        }
        catch (OrderNotFoundException)
        {
            return NotFound();
        }
        catch (DomainException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    private ModelStateDictionary ToModelState(ValidationResult validation)
    {
        foreach (var error in validation.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        return ModelState;
    }
}
