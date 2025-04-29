namespace Learning.DTO;

/// <summary>
/// DTO for Http answer containing data and other possible endpoints.
/// Used to comply with Uniform interface principle of REST
/// </summary>
/// <typeparam name="TValue">Type of payload value</typeparam>
public record Dto<TValue>
{
    /// <summary>
    /// Links to other related endpoints
    /// </summary>
    public IEnumerable<HttpLink>? Links { get; init; }
    
    /// <summary>
    /// Main response data
    /// </summary>
    public TValue? Data { get; init; }

    public Dto()
    {
        
    }
    
    public Dto(TValue data, IEnumerable<HttpLink> links)
    {
        Data = data;
        Links = links;
    }

    public Dto(TValue data, params HttpLink[] links)
    {
        Data = data;
        Links = links;
    }
}