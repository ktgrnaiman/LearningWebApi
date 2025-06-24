using System.Diagnostics.CodeAnalysis;

namespace Learning.DTO;

/// <summary>
/// Links for DTO in order to inform client with possible operations.
/// Used to comply with Uniform interface principle of REST.
/// </summary>
public record HttpLink
{
    /// <summary>
    /// Reference to endpoint
    /// </summary>
    public string Ref { get; init; }
    
    /// <summary>
    /// Type of Http request
    /// </summary>
    public string Type { get; init; }
    
    /// <summary>
    /// Relation to specified Http resource
    /// </summary>
    public string Rel { get; init; }
   
    /// <param name="reference">Reference to endpoint</param>
    /// <param name="type">Type of Http request</param>
    /// <param name="relation">Relation to specified Http resource</param>
    public HttpLink(string reference, string relation, string type)
    {
        Ref = reference ?? throw new ArgumentNullException(nameof(reference));
        Rel = relation ?? throw new ArgumentNullException(nameof(relation));
        Type = type ?? throw new ArgumentNullException(nameof(type));
    }
}