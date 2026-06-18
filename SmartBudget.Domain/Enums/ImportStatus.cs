using System.Text.Json.Serialization;

namespace SmartBudget.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ImportStatus
{
    PENDING,
    PROCESSING,
    COMPLETED,
    FAILED
}