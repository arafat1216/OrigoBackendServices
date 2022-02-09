namespace CustomerServices.ServiceModels
{
    public class OktaUserDTO
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public OktaUserProfile Profile { get; set; }
    }

    public class OktaUserProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MytosSubsId { get; set; }
        public string MobilePhone { get; set; }
        public string TechstepUserId { get; set; }
        public string DisplayName { get; set; }
        public string OrganizationNumber { get; set; }
        public string AgentSalesId { get; set; }
        public int WebshopPolicy { get; set; }
        public string SecondEmail { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
    }
}
