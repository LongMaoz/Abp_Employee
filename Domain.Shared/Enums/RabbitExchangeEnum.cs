using Domain.Shared.Attributes;

namespace Domain.Shared.Enums;

public enum RabbitExchangeEnum
{
    [EnumString("employee.events")]
    Employee
}