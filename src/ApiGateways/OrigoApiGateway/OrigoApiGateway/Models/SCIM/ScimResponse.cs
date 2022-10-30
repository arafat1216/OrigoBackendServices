namespace OrigoApiGateway.Models.SCIM;

/// <summary>
/// Class that wraps SCIM resources such as e.g. a <see cref="ScimUser"/> in metadata required by the SCIM protocol
/// </summary>
public class ScimResponse<T>
{
    public string Schemas { get; set; } = "urn:ietf:params:scim:api:messages:2.0:ListResponse";
    public int TotalResults { get; set; } = 0;
    public int StartIndex { get; set; } = 1;
    public List<T> Resources { get; set; }

    public ScimResponse (T resource)
    {
        TotalResults = 1;
        Resources = new List<T>() { resource };
    }

    public ScimResponse (List<T> resources)
    {
        Resources = resources;
        TotalResults = Resources.Count;
    }

    public void AddResource(T resource)
    {
        Resources.Add(resource);
        TotalResults++;
    }
}
