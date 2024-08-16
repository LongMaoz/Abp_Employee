using Domain.Shared.Attributes;

namespace Domain.Shared.Enums;

public enum RabbitRoutingKeyEnum
{
    [EnumString("employee.inserted")]
    EmployeeInserted,
    [EnumString("employee.updated")]
    EmployeeUpdated,
    [EnumString("employee.deleted")]
    EmployeeDeleted,
}