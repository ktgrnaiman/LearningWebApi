namespace Learning.DTO;

/// <summary>
/// DTO for Http answer containing data and other possible endpoints.
/// Used to comply with Uniform interface principle of REST
/// </summary>
/// <typeparam name="TValue">Type of payload value</typeparam>
public record ResponseDto<TValue>
{
    /// <summary>
    /// Links to other related endpoints
    /// </summary>
    public IEnumerable<HttpLink>? Links { get; init; }
    
    /// <summary>
    /// Index of requested page
    /// </summary>
    public int? PageIndex { get; set; }
    
    /// <summary>
    /// Size of requested page
    /// </summary>
    public int? PageSize { get; set; }
    
    /// <summary>
    /// Amount of elements of requested type
    /// </summary>
    public int? EntriesCount { get; set; }
    
    /// <summary>
    /// Main response data
    /// </summary>
    public TValue? Data { get; init; }
    
    public ResponseDto()
    {
        
    }
    
    public ResponseDto(TValue data, int pageIndex, int pageSize, IEnumerable<HttpLink> links)
    {
        Data = data;
        Links = links;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    public ResponseDto(TValue data, int pageIndex, int pageSize, params HttpLink[] links)
    {
        Data = data;
        Links = links;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }
}