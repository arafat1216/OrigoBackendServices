namespace Common.Model.EventModels;

public record UserChangedDepartmentEvent : IUserEvent
{
    public Guid CustomerId { get; set; }
    public Guid UserId { get; set; }
    public Guid? DepartmentId { get; set; }
    public DateTime CreatedDate { get; set; }
}