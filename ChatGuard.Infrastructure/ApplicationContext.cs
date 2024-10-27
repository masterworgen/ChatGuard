using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using ChatGuard.Domain.Entity;

namespace ChatGuard.Infrastructure;

public class ApplicationContext : DbContext
{
    public DbSet<Chat> Chats => Set<Chat>();

    public ApplicationContext(IConfiguration configuration, DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}