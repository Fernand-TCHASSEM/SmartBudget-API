namespace SmartBudget.Domain.Interfaces;

public interface IDataSeeder
{
    Task SeedAsync(CancellationToken ct = default);
}
