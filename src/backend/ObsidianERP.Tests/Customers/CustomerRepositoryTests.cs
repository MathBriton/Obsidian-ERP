using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ObsidianERP.Domain.Entities;
using ObsidianERP.Infrastructure.Persistence;
using ObsidianERP.Infrastructure.Persistence.Repositories;

namespace ObsidianERP.Tests.Customers;

public class CustomerRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _db;

    public CustomerRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;
        _db = new ApplicationDbContext(options);
        _db.Database.EnsureCreated();
    }

    [Fact]
    public async Task AddAsync_persiste_e_GetByIdAsync_recupera()
    {
        var repo = new CustomerRepository(_db);
        var customer = new Customer { Name = "Acme" };

        await repo.AddAsync(customer);
        await repo.SaveChangesAsync();

        (await repo.GetByIdAsync(customer.Id)).Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_nao_retorna_cliente_soft_deleted()
    {
        var repo = new CustomerRepository(_db);
        var customer = new Customer { Name = "Acme", IsDeleted = true };

        await repo.AddAsync(customer);
        await repo.SaveChangesAsync();

        (await repo.GetByIdAsync(customer.Id)).Should().BeNull();
    }

    [Fact]
    public async Task GetPagedAsync_aplica_busca_case_insensitive_e_paginacao()
    {
        var repo = new CustomerRepository(_db);
        foreach (var name in new[] { "Acme", "Acme Brasil", "Globex" })
        {
            await repo.AddAsync(new Customer { Name = name });
        }
        await repo.SaveChangesAsync();

        var (items, total) = await repo.GetPagedAsync(1, 10, "acme", default);

        total.Should().Be(2);
        items.Should().OnlyContain(c => c.Name.Contains("Acme"));
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}
