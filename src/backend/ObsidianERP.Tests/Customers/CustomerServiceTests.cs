using FluentAssertions;
using NSubstitute;
using ObsidianERP.Application.Abstractions;
using ObsidianERP.Application.DTOs;
using ObsidianERP.Application.Exceptions;
using ObsidianERP.Application.Services;
using ObsidianERP.Domain.Entities;

namespace ObsidianERP.Tests.Customers;

public class CustomerServiceTests
{
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();

    private CustomerService CreateSut() => new(_repository);

    [Fact]
    public async Task CreateAsync_adiciona_e_retorna_o_dto()
    {
        var dto = await CreateSut().CreateAsync(
            new CreateCustomerRequest("Acme", "acme@x.com", "11999", "123"));

        dto.Name.Should().Be("Acme");
        dto.Email.Should().Be("acme@x.com");
        await _repository.Received(1).AddAsync(
            Arg.Is<Customer>(c => c.Name == "Acme"), Arg.Any<CancellationToken>());
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdAsync_inexistente_lanca_CustomerNotFound()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Customer?)null);

        await CreateSut()
            .Invoking(s => s.GetByIdAsync(Guid.NewGuid()))
            .Should().ThrowAsync<CustomerNotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_existente_atualiza_os_campos()
    {
        var existing = new Customer { Name = "Antigo" };
        _repository.GetByIdAsync(existing.Id, Arg.Any<CancellationToken>()).Returns(existing);

        var dto = await CreateSut().UpdateAsync(
            existing.Id, new UpdateCustomerRequest("Novo", "novo@x.com", null, null));

        dto.Name.Should().Be("Novo");
        existing.Name.Should().Be("Novo");
        _repository.Received(1).Update(existing);
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_inexistente_lanca_CustomerNotFound()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Customer?)null);

        await CreateSut()
            .Invoking(s => s.UpdateAsync(Guid.NewGuid(), new UpdateCustomerRequest("X", null, null, null)))
            .Should().ThrowAsync<CustomerNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_marca_como_soft_deleted()
    {
        var existing = new Customer { Name = "Acme" };
        _repository.GetByIdAsync(existing.Id, Arg.Any<CancellationToken>()).Returns(existing);

        await CreateSut().DeleteAsync(existing.Id);

        existing.IsDeleted.Should().BeTrue();
        existing.DeletedAt.Should().NotBeNull();
        _repository.Received(1).Update(existing);
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPagedAsync_mapeia_o_resultado_paginado()
    {
        IReadOnlyList<Customer> items = [new() { Name = "A" }, new() { Name = "B" }];
        _repository.GetPagedAsync(1, 10, null, Arg.Any<CancellationToken>()).Returns((items, 2));

        var result = await CreateSut().GetPagedAsync(new CustomerQuery(1, 10, null));

        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Page.Should().Be(1);
    }
}
