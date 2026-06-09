namespace SmartBudget.Domain.Interfaces;

public interface IDataSeeder
{
    int Order { get; }
    Task SeedAsync(CancellationToken ct = default);
}
