namespace SmartBudget.Domain.Interfaces;

public interface ISoftDeletable
{
    DateTime? DeletedAt { get; set; }
}
