using System.Linq.Dynamic.Core;
using System.Reflection.Metadata;
using System.Text.Json;
using Learning.Constants;
using Learning.DTO;
using Learning.Extensions;
using Learning.Models;
using Learning.ServiceRegister;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Learning.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MechanicController(
    ApplicationDbContext context, 
    ILogger<MechanicController> logger, 
    DistributedCacheProvider cacheProvider) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<MechanicController> _logger = logger;
    private readonly IDistributedCache _cache = cacheProvider.GetRequired("Redis");
    
    /// <summary>
    /// Action for getting array of Mechanic records
    /// </summary>
    /// <param name="request">Full request spec with pagination, sorting and filtering</param>
    /// <returns>Page of specified Mechanics records</returns>
    [HttpGet("GetMechanics")]
    [ResponseCache(CacheProfileName = "NoCache")]
    public async Task<ResponseDto<Mechanic[]>> GetMechanics([FromQuery] GetRequestDto<Mechanic> request)
    {
        _logger.LogInformation(CustomLogEvents.BoardGameGet, "GetMechanics method started at {HH:mm:ss}", DateTime.UtcNow);
        
        IQueryable<Mechanic> query = _context.Mechanics;

        if (request.FilterQuery is not null)
            query = query.Where(m => m.Name.Contains(request.FilterQuery));
        int count = await query.CountAsync();

        Mechanic[] result;
        string key = $"{nameof(GetRequestDto<Mechanic>)}-{JsonSerializer.Serialize(request)}";

        if (!_cache.TryGetValue(key, out result!))
        {
            //TODO:Change when binding for complex type will be implemented
            if (!string.IsNullOrWhiteSpace(request.SortColumn))
            {
                string orderDir = request.SortDir ?? "ASC";
                query = query.OrderBy($"{request.SortColumn} {orderDir}");
            }
            query = query.Skip(request.PageIndex * request.PageSize).Take(request.PageSize);
            result = await query.ToArrayAsync();
            _cache.Set(key, result, new TimeSpan(0, 0, 30));
            
            _logger.LogInformation("Distributed cache entry has been created");
        }
        else _logger.LogInformation("Distributed cache entry has been retrieved");

        return new ResponseDto<Mechanic[]>()
        {
            Data = result,
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
    [ResponseCache(CacheProfileName = "NoCache")]
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
    [ResponseCache(CacheProfileName = "NoCache")]
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