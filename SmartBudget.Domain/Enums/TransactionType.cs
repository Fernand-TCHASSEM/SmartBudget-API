using System.Text.Json.Serialization;

namespace SmartBudget.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransactionType
{
    DEBIT,
    CREDIT
}