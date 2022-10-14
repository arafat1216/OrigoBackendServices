namespace CustomerServices.ServiceModels;

public record DepartmentNamesDTO
{
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; }
}