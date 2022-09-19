
using Common.Configuration;

namespace CustomerServices.Email.Configuration
{
    public record EmailConfigurationCustomer : EmailConfiguration
    {
        public string LoginPath { get; set; }
        public string UserDetailViewPath { get; set; }
        public string MyPagePath { get; set; }
    }
}
