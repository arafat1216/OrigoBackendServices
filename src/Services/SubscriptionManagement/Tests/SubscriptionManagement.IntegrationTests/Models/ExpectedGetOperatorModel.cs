namespace SubscriptionManagement.IntegrationTests.Controllers;

public record ExpectedGetOperatorModel
{
    public string Name { get; set; }
    public string Country { get; set; }
    public int Id { get; set; }
}