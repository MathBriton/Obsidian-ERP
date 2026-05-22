using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ObsidianERP.Domain.Entities;
using ObsidianERP.Infrastructure.Persistence;
using ObsidianERP.Infrastructure.Persistence.Repositories;

namespace ObsidianERP.Tests.Auth;

public class UserRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _db;

    public UserRepositoryTests()
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
    public async Task AddAsync_persiste_e_GetByEmailAsync_recupera_o_usuario()
    {
        var repo = new UserRepository(_db);
        await repo.AddAsync(new User { Name = "Ana", Email = "ana@x.com", PasswordHash = "hash" });
        await repo.SaveChangesAsync();

        var found = await repo.GetByEmailAsync("ana@x.com");

        found.Should().NotBeNull();
        found!.Name.Should().Be("Ana");
    }

    [Fact]
    public async Task EmailExistsAsync_retorna_true_quando_o_email_existe()
    {
        var repo = new UserRepository(_db);
        await repo.AddAsync(new User { Name = "Ana", Email = "ana@x.com", PasswordHash = "hash" });
        await repo.SaveChangesAsync();

        (await repo.EmailExistsAsync("ana@x.com")).Should().BeTrue();
        (await repo.EmailExistsAsync("outro@x.com")).Should().BeFalse();
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}
