namespace OrigoApiGateway.Models.SCIM;

/// <summary>
/// Class that wraps SCIM resources such as e.g. a <see cref="ScimUser"/> in metadata required by the SCIM protocol
/// </summary>
public class ListResponse
{
    public string Schemas { get; set; } = "urn:ietf:params:scim:api:messages:2.0:ListResponse";
    public int TotalResults { get; set; } = 0;
    public int StartIndex { get; set; } = 1; // TODO: Necessary, or removable?
    public int ItemsPerPage { get; set; } = 25; // TODO: Necessary, or removable?
    public List<Resource> Resources { get; set; } = new();

    public ListResponse (Resource resource)
    {
        TotalResults = 1;
        Resources = new List<Resource>() { resource };
    }

    public ListResponse (List<Resource> resources)
    {
        Resources = resources;
        TotalResults = Resources.Count;
    }

    public void AddResource (Resource resource)
    {
        Resources.Add(resource);
        TotalResults++;
    }

    public void RemoveResource (Resource resource)
    { 
        Resources.Remove(resource);
        TotalResults--;
    }

    public void AddResources (List<Resource> resources)
    {
        Resources.AddRange(resources);
        TotalResults = Resources.Count;
    }

    public void RemoveResources(List<Resource> resources)
    {
        Resources.RemoveAll(resource => resources.Contains(resource));
        TotalResults = Resources.Count;
    }
}
