namespace Customer.API.ViewModels
{
    public record ContactPerson
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

    }
}