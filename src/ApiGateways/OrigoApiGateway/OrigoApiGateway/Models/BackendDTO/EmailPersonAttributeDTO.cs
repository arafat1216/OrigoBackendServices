namespace OrigoApiGateway.Models.BackendDTO
{
    public class EmailPersonAttributeDTO
    {
        [EmailAddress]
        public string Email { get; set; }

        public string Name { get; set; }

        // TODO: Should this be enforced to 2-characters to follow the ISO standard?
        public string PreferedLanguage { get; set; }
    }
}
