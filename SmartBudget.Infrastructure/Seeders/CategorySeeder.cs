using Microsoft.EntityFrameworkCore;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Interfaces;
using SmartBudget.Infrastructure.Persistence;

namespace SmartBudget.Infrastructure.Seeders;

public class CategorySeeder(SmartBudgetDbContext db) : IDataSeeder
{
    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await db.Categories.AnyAsync(c => c.IsDefault, ct)) return;

        db.Categories.AddRange(
            new Category { Name = "Alimentation",   Icon = "🛒", Color = "#10B981", IsDefault = true, IsIncome = false, SortOrder =  1 },
            new Category { Name = "Transport",      Icon = "🚗", Color = "#3B82F6", IsDefault = true, IsIncome = false, SortOrder =  2 },
            new Category { Name = "Logement",       Icon = "🏠", Color = "#8B5CF6", IsDefault = true, IsIncome = false, SortOrder =  3 },
            new Category { Name = "Loisirs",        Icon = "🎮", Color = "#F59E0B", IsDefault = true, IsIncome = false, SortOrder =  4 },
            new Category { Name = "Santé",          Icon = "💊", Color = "#EF4444", IsDefault = true, IsIncome = false, SortOrder =  5 },
            new Category { Name = "Vêtements",      Icon = "👕", Color = "#EC4899", IsDefault = true, IsIncome = false, SortOrder =  6 },
            new Category { Name = "Restaurants",    Icon = "🍽️", Color = "#F97316", IsDefault = true, IsIncome = false, SortOrder =  7 },
            new Category { Name = "Épargne",        Icon = "💰", Color = "#14B8A6", IsDefault = true, IsIncome = false, SortOrder =  8 },
            new Category { Name = "Impôts & Frais", Icon = "🏛️", Color = "#6B7280", IsDefault = true, IsIncome = false, SortOrder =  9 },
            new Category { Name = "Shopping",       Icon = "🛍️", Color = "#A855F7", IsDefault = true, IsIncome = false, SortOrder = 10 },
            new Category { Name = "Revenus",        Icon = "📥", Color = "#22C55E", IsDefault = true, IsIncome = true,  SortOrder = 11 },
            new Category { Name = "Autre",          Icon = "❓", Color = "#9CA3AF", IsDefault = true, IsIncome = false, SortOrder = 99 }
        );

        await db.SaveChangesAsync(ct);
    }
}
