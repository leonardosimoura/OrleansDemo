using Microsoft.EntityFrameworkCore;

namespace MSOrleansDemo.Context
{
    public static class EFMigrator
    {
        public static void ExecuteEntityFrameworkMigrations(this IApplicationBuilder builder)
        {
            using var scope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var ctx = scope.ServiceProvider.GetRequiredService<OrleansDemoDbContext>();

            var pendingMigrations = ctx.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                ctx.Database.Migrate();
            }
        }
    }
}