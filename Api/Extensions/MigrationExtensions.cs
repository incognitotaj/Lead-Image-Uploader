using Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Api.Extensions;

/// <summary>
/// Apply migrations to the database using EF Core
/// </summary>
public static class MigrationExtensions
{
    /// <summary>
    /// Apply migrations
    /// </summary>
    /// <param name="app"></param>
    public static async Task ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}
