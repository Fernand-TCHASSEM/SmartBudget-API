using Microsoft.EntityFrameworkCore;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Interfaces;
using SmartBudget.Infrastructure.Persistence;

namespace SmartBudget.Infrastructure.Seeders;

public class CategoryRuleSeeder(SmartBudgetDbContext db) : IDataSeeder
{
    public int Order => 2;

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await db.CategoryRules.AnyAsync(c => c.UserId == null, ct)) return;

        db.CategoryRules.AddRange(
            // Alimentation
            new CategoryRule { Id = "rul-001", CategoryId = "cat-001", Keyword = "TIM HORTONS", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-002", CategoryId = "cat-001", Keyword = "METRO", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-003", CategoryId = "cat-001", Keyword = "SOBEYS", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-004", CategoryId = "cat-001", Keyword = "MAXI", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-005", CategoryId = "cat-001", Keyword = "IGA", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-006", CategoryId = "cat-001", Keyword = "WALMART", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-007", CategoryId = "cat-001", Keyword = "LOBLAWS", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-008", CategoryId = "cat-001", Keyword = "PROVIGO", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-009", CategoryId = "cat-001", Keyword = "COSTCO", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-010", CategoryId = "cat-002", Keyword = "PRESTO", IsRegex = false, Priority = 10 },
            // Transport
            new CategoryRule { Id = "rul-011", CategoryId = "cat-002", Keyword = "OC TRANSPO", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-012", CategoryId = "cat-002", Keyword = "UBER", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-013", CategoryId = "cat-002", Keyword = "ESSO", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-014", CategoryId = "cat-002", Keyword = "SHELL", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-015", CategoryId = "cat-002", Keyword = "PETRO-CANADA", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-016", CategoryId = "cat-004", Keyword = "NETFLIX", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-017", CategoryId = "cat-004", Keyword = "SPOTIFY", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-018", CategoryId = "cat-004", Keyword = "STEAM", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-019", CategoryId = "cat-004", Keyword = "AMAZON PRIME", IsRegex = false, Priority = 10 },
            // Logement
            new CategoryRule { Id = "rul-020", CategoryId = "cat-003", Keyword = "ROGERS", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-021", CategoryId = "cat-003", Keyword = "BELL", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-022", CategoryId = "cat-003", Keyword = "HYDRO", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-023", CategoryId = "cat-003", Keyword = "ENBRIDGE", IsRegex = false, Priority = 10 },
            // Restaurants
            new CategoryRule { Id = "rul-024", CategoryId = "cat-007", Keyword = "DOORDASH", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-025", CategoryId = "cat-007", Keyword = "UBER EATS", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-026", CategoryId = "cat-007", Keyword = "SKIP", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-027", CategoryId = "cat-007", Keyword = "MCDONALD", IsRegex = false, Priority = 10 },
            // Achats
            new CategoryRule { Id = "rul-028", CategoryId = "cat-010", Keyword = "AMAZON", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-029", CategoryId = "cat-010", Keyword = "BEST BUY", IsRegex = false, Priority = 10 },
            new CategoryRule { Id = "rul-030", CategoryId = "cat-010", Keyword = "CANADIAN TIRE", IsRegex = false, Priority = 10 },
            // Revenus
            new CategoryRule { Id = "rul-031", CategoryId = "cat-011", Keyword = "PAYROLL", IsRegex = false, Priority = 5 },
            new CategoryRule { Id = "rul-032", CategoryId = "cat-011", Keyword = "DIRECT DEPOSIT", IsRegex = false, Priority = 5 },
            new CategoryRule { Id = "rul-033", CategoryId = "cat-011", Keyword = "E-TRANSFER", IsRegex = false, Priority = 20 }
        );

        await db.SaveChangesAsync(ct);
    }
}
