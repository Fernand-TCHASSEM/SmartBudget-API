using System.Text.Json.Serialization;

namespace SmartBudget.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccountType
{
    CHEQUING,
    SAVINGS,
    CREDIT
}