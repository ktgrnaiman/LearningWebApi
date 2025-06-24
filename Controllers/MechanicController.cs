using System.Linq.Dynamic.Core;
using System.Reflection.Metadata;
using Learning.DTO;
using Learning.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Learning.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MechanicController(ApplicationDbContext context, ILogger<MechanicController> logger) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<MechanicController> _logger = logger;

    /// <summary>
    /// Action for getting array of Mechanic records
    /// </summary>
    /// <param name="request">Full request spec with pagination, sorting and filtering</param>
    /// <returns>Page of specified Mechanics records</returns>
    [HttpGet("GetMechanic")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    public async Task<ResponseDto<Mechanic[]>> GetMechanics([FromQuery] GetRequestDto<Mechanic> request)
    {
        IQueryable<Mechanic> query = _context.Mechanics;

        if (request.FilterQuery is not null)
            query = query.Where(m => m.Name.Contains(request.FilterQuery));
        int count = await query.CountAsync();
        
        if (request.SortColumn is not null)
            query = query.OrderBy($"{request.SortColumn} {request.SortDir}");

        query = query.Skip(request.PageIndex * request.PageSize).Take(request.PageIndex);

        return new ResponseDto<Mechanic[]>()
        {
            Data = await query.ToArrayAsync(),
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            EntriesCount = count,
            Links = [new HttpLink(
                Url.Action(null, "Mechanic", new {request.PageIndex, request.PageSize}, Request.Scheme)!, 
                "self", 
                "GET"
                )
            ]
        };
    }
    
    /// <summary>
    /// Update existing Mechanic record
    /// </summary>
    /// <param name="update">Update data</param>
    /// <returns>Updated Mechanic record</returns>
    [HttpPost("UpdateMechanic")]
    [ResponseCache(NoStore = true)]
    public async Task<ResponseDto<Mechanic?>> UpdateMechanic(MechanicDto update)
    {
        var mechanic = await _context.Mechanics.FindAsync(update.Id);

        if (mechanic is not null)
        {
            mechanic.Name = update.Name;
            mechanic.LastModifiedDate = DateTime.UtcNow;
            _context.Mechanics.Update(mechanic);
            await _context.SaveChangesAsync();
        }

        return new ResponseDto<Mechanic?>()
        {
            Data = mechanic,
            Links = [new HttpLink(
                    Url.Action(null, "Mechanic", update, Request.Scheme)!,
                    "self",
                    "POST"
                )
            ]
        };
    }
    
    /// <summary>
    /// Deletes a Mechanic record with specified id
    /// </summary>
    /// <param name="id">ID of Mechanic record to delete</param>
    /// <returns>Deleted Mechanic record</returns>
    [HttpDelete("DeleteMechanic")]
    [ResponseCache(NoStore = true)]
    public async Task<ResponseDto<Mechanic?>> DeleteMechanic(int id)
    {
        var mechanic = await _context.Mechanics.FindAsync(id);

        if (mechanic is not null)
        {
            _context.Mechanics.Remove(mechanic);
            await _context.SaveChangesAsync();
        }

        return new ResponseDto<Mechanic?>()
        {
            Data = mechanic,
            Links = [new HttpLink(
                    Url.Action(null, "Mechanic", id, Request.Scheme)!,
                    "self",
                    "DELETE"
                )
            ]
        };
    }
}