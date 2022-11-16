using System.Text.Json.Serialization;

namespace OrigoApiGateway.Models.SCIM;

/// <summary>
/// Class that wraps SCIM resources such as e.g. a <see cref="ScimUser"/> in metadata required by the SCIM protocol
/// </summary>
public class ListResponse<T>
{
    public List<string> Schemas { get; set; } = new() { "urn:ietf:params:scim:api:messages:2.0:ListResponse" };
    public int TotalResults { get; set; } = 0;
    public int StartIndex { get; set; } = 1;
    public int ItemsPerPage { get; set; } = 25;

    [JsonPropertyName("Resources")]
    public List<T> Resources { get; set; } = new();

    public ListResponse(T resource)
    {
        TotalResults = 1;
        Resources = new List<T>() { resource };
    }

    public ListResponse(List<T> resources)
    {
        Resources = resources;
        TotalResults = Resources.Count;
    }

    public void AddResource(T resource)
    {
        Resources.Add(resource);
        TotalResults++;
    }

    public void RemoveResource(T resource)
    {
        Resources.Remove(resource);
        TotalResults--;
    }

    public void AddResources(List<T> resources)
    {
        Resources.AddRange(resources);
        TotalResults = Resources.Count;
    }

    public void RemoveResources(List<T> resources)
    {
        Resources.RemoveAll(resource => resources.Contains(resource));
        TotalResults = Resources.Count;
    }
}
