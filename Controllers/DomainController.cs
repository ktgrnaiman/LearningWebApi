using System.Linq.Dynamic.Core;
using System.Runtime.InteropServices.JavaScript;
using Learning.Attributes;
using Learning.DTO;
using Learning.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Learning.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DomainController(ApplicationDbContext context, ILogger<DomainController> logger) : ControllerBase
{
    private ILogger<DomainController> _logger = logger;
    private ApplicationDbContext _context = context;
    
    /// <summary>
    /// Action for getting array of Domain records
    /// </summary>
    /// <param name="request">Full request spec with pagination, sorting and filtering</param>
    /// <returns>Page of specified Domains records</returns>
    [HttpGet("GetDomains")]
    [ResponseCache(CacheProfileName = "Any60")]
    [ManualValidationFilter]
    public async Task<ActionResult<ResponseDto<Domain[]>>> GetDomains([FromQuery] GetRequestDto<Domain> request)
    {
        IQueryable<Domain> query = _context.Domains;
        
        if (!string.IsNullOrWhiteSpace(request.FilterQuery))
            query = query.Where(d => d.Name.Contains(request.FilterQuery));
        int count = await query.CountAsync();
        
        if (request.SortColumn is not null)
            query = query.OrderBy($"{request.SortColumn} {request.SortDir}");

        query = query.Skip(request.PageIndex * request.PageSize).Take(request.PageSize);

        return new ResponseDto<Domain[]>()
        {
            PageIndex = request.PageIndex,
            PageSize = request.PageIndex,
            EntriesCount = count,
            Data = await query.ToArrayAsync(),
            Links = [new HttpLink(
                Url.Action(null, "Domain", new {request.PageIndex, request.PageSize}, Request.Scheme)!, 
                "self", 
                "GET"
                )
            ]
        };
    }

    private ActionResult? HandleModelState()
    {
        if (!ModelState.IsValid)
        {
            var details = new ValidationProblemDetails(ModelState);
            details.Extensions["traceId"] = System.Diagnostics.Activity.Current?.Id ??
                                            HttpContext.TraceIdentifier;
            
            if (ModelState.Keys.Any(s => s == "Id" || s == "Name"))
            {
                details.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3";
                details.Status = StatusCodes.Status403Forbidden;
                return new ObjectResult(details) {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
            
            details.Type = "https:/ /tools.ietf.org/html/rfc7231#section-6.5.1";
            details.Status = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(details);
        }
        return null;
    }
    
    /// <summary>
    /// Update existing Domain record
    /// </summary>
    /// <param name="update">Update data</param>
    /// <returns>Updated Domain record</returns>
    [HttpPost("UpdateDomain")]
    [ResponseCache(CacheProfileName = "NoCache")]
    public async Task<ResponseDto<Domain>> PostDomain(DomainDto update)
    {
        var domain = await _context.Domains.FindAsync(update.Id);

        if (domain is not null)
        {
            domain.Name = update.Name;
            domain.LastModifiedDate = DateTime.UtcNow;
            _context.Domains.Update(domain);
            await _context.SaveChangesAsync();
        }

        return new ResponseDto<Domain>()
        {
            Data = domain,
            Links = [new HttpLink(
                Url.Action(null, "Domain", update, Request.Scheme)!, 
                "self", 
                "POST"
                )
            ]
        };
    }
    
    /// <summary>
    /// Deletes a Mechanic record with specified id
    /// </summary>
    /// <param name="id">ID of Domain record to delete</param>
    /// <returns>Deleted Domain record</returns>
    [HttpDelete("DeleteDomain")]
    [ResponseCache(CacheProfileName = "NoCache")]
    public async Task<ResponseDto<Domain>> DeleteDomain(int id)
    {
        var domain = await _context.Domains.FindAsync(id);

        if (domain is not null)
        {
            _context.Domains.Remove(domain);
            await _context.SaveChangesAsync();
        }

        return new ResponseDto<Domain>()
        {
            Data = domain,
            Links = [new HttpLink(
                Url.Action(null, "Domain", id, Request.Scheme)!, 
                "self", 
                "DELETE"
                )
            ]
        };
    }
}