using Microsoft.EntityFrameworkCore;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Interfaces;
using SmartBudget.Infrastructure.Persistence;

namespace SmartBudget.Infrastructure.Seeders;

public class CategorySeeder(SmartBudgetDbContext db) : IDataSeeder
{
    public int Order => 1;

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await db.Categories.AnyAsync(c => c.IsDefault, ct)) return;

        db.Categories.AddRange(
            new Category { Id = "cat-001", Name = "Alimentation", Icon = "🛒", Color = "#10B981", IsDefault = true, IsIncome = false, SortOrder = 1 },
            new Category { Id = "cat-002", Name = "Transport", Icon = "🚗", Color = "#3B82F6", IsDefault = true, IsIncome = false, SortOrder = 2 },
            new Category { Id = "cat-003", Name = "Logement", Icon = "🏠", Color = "#8B5CF6", IsDefault = true, IsIncome = false, SortOrder = 3 },
            new Category { Id = "cat-004", Name = "Loisirs", Icon = "🎮", Color = "#F59E0B", IsDefault = true, IsIncome = false, SortOrder = 4 },
            new Category { Id = "cat-005", Name = "Santé", Icon = "💊", Color = "#EF4444", IsDefault = true, IsIncome = false, SortOrder = 5 },
            new Category { Id = "cat-006", Name = "Vêtements", Icon = "👕", Color = "#EC4899", IsDefault = true, IsIncome = false, SortOrder = 6 },
            new Category { Id = "cat-007", Name = "Restaurants", Icon = "🍽️", Color = "#F97316", IsDefault = true, IsIncome = false, SortOrder = 7 },
            new Category { Id = "cat-008", Name = "Épargne", Icon = "💰", Color = "#14B8A6", IsDefault = true, IsIncome = false, SortOrder = 8 },
            new Category { Id = "cat-009", Name = "Impôts & Frais", Icon = "🏛️", Color = "#6B7280", IsDefault = true, IsIncome = false, SortOrder = 9 },
            new Category { Id = "cat-010", Name = "Shopping", Icon = "🛍️", Color = "#A855F7", IsDefault = true, IsIncome = false, SortOrder = 10 },
            new Category { Id = "cat-011", Name = "Revenus", Icon = "📥", Color = "#22C55E", IsDefault = true, IsIncome = true, SortOrder = 11 },
            new Category { Id = "cat-012", Name = "Autre", Icon = "❓", Color = "#9CA3AF", IsDefault = true, IsIncome = false, SortOrder = 99 }
        );

        await db.SaveChangesAsync(ct);
    }
}
