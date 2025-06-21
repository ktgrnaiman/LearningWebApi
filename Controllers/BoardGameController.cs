using Microsoft.AspNetCore.Mvc;
using Learning.DTO;
using Learning.Models;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Learning.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardGameController : ControllerBase
{
    private ILogger<BoardGameController> _logger;
    private ApplicationDbContext _context;
    
    public BoardGameController(ApplicationDbContext context, ILogger<BoardGameController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    /// <summary>
    /// Returns paginated entries in DTO
    /// </summary>
    /// <param name="pageIndex">Index of requested page</param>
    /// <param name="pageSize">Size of pages to use in calculation and data retrieval</param>
    /// <param name="filterQuery">Query string for comparing with names</param>
    /// <param name="sortColumn">Name of column used as criteria for sorting</param>
    /// <param name="sortAsc">Ascending or descending sorting</param>
    /// <returns>Array of board games in DTO</returns>
    [HttpGet("Get")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    public async Task<Dto<BoardGame[]>> Get(int pageIndex = 0, int pageSize = 10, string? filterQuery = null, string? sortColumn = null, bool sortAsc = true)
    {
        IQueryable<BoardGame> query = _context.BoardGames;

        if (!String.IsNullOrWhiteSpace(filterQuery))
            query = query.Where(game => game.Name.Contains(filterQuery));
        int entryCount = await query.CountAsync();
        
        if (!String.IsNullOrWhiteSpace(sortColumn))
        {
            string orderDir = sortAsc ? "ASC" : "DESC";
            query = query.OrderBy($"{sortColumn} {orderDir}");
        }
        
        query = query.Skip(pageIndex * pageSize).Take(pageSize); 
        
        return new Dto<BoardGame[]>()
        {
            Data = await query.ToArrayAsync(),
            PageIndex = pageIndex,
            PageSize = pageSize,
            EntriesCount = entryCount,
            Links = [new HttpLink(
                Url.Action(null, "BoardGame", new {pageIndex, pageSize}, Request.Scheme)!,
                "GET", "self"
            )]
        };
    }
}