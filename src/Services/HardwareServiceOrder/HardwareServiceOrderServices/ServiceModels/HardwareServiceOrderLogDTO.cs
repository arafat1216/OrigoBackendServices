namespace HardwareServiceOrderServices.ServiceModels;

public class HardwareServiceOrderLogDTO
{
    public DateTime Timestamp { get; set; }
    public string Status { get; set; }
    public string ServiceProvider { get; set; }
}