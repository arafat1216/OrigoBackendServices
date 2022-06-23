namespace OrigoApiGateway.Models.Asset
{
    public class AssignAssetToUserDTO
    {
        public Guid AssetId { get; set; }
        public Guid UserId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid UserAssigneToDepartment { get; set; }
        public Guid CallerId { get; set; }
    }
}
