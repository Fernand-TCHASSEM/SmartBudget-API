namespace SmartBudget.Domain.Interfaces;

public interface IHasTimestamps
{
    DateTime UpdatedAt { get; set; }
}
