using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Learning.DTO;
using Learning.Models;
using Learning.Attributes;
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
    /// <param name="sortDir">Ascending or descending sorting</param>
    /// <returns>Array of board games in DTO</returns>
    [HttpGet("GetGame")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    public async Task<Dto<BoardGame[]>> GetGame([FromQuery]GetRequestDto<BoardGameDto> request)
    {
        IQueryable<BoardGame> query = _context.BoardGames;

        if (!string.IsNullOrWhiteSpace(request.FilterQuery))
            query = query.Where(game => game.Name.Contains(request.FilterQuery));
        int entryCount = await query.CountAsync();
        
        if (!string.IsNullOrWhiteSpace(request.SortColumn))
        {
            string orderDir = request.SortDir ?? "ASC";
            query = query.OrderBy($"{request.SortColumn} {orderDir}");
        }
        
        query = query.Skip(request.PageIndex * request.PageSize).Take(request.PageSize); 
        
        return new Dto<BoardGame[]>()
        {
            Data = await query.ToArrayAsync(),
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            EntriesCount = entryCount,
            Links = [new HttpLink(
                Url.Action(null, "BoardGame", new {request.PageIndex, request.PageSize}, Request.Scheme)!,
                "self", "GET")
            ]
        };
    }
    
    /// <summary>
    /// Updates board game's name, year properties
    /// </summary>
    /// <param name="updateGame">New values for updating</param>
    /// <returns>Updated board game entry</returns>
    [HttpPost("UpdateGame")]
    [ResponseCache(NoStore = true)]
    public async Task<Dto<BoardGame?>> PostGame(BoardGameDto updateGame)
    {
        var game = await _context.BoardGames.FindAsync(updateGame.Id);

        if (game is not null)
        {
            if (!string.IsNullOrWhiteSpace(updateGame.Name))
                game.Name = updateGame.Name;
            if (updateGame.Year is > 0)
                game.Year = updateGame.Year.Value;
            game.LastModifiedDate = DateTime.UtcNow;
            _context.Update(game);
            await _context.SaveChangesAsync();
        }

        return new Dto<BoardGame?>()
        {
            Data = game,
            Links = [new HttpLink(
                Url.Action(null, "BoardGame", updateGame, Request.Scheme)!, 
                "self", "POST")
            ]
        };
    }
    
    /// <summary>
    /// Deletes board game entry with specified unique ID
    /// </summary>
    /// <param name="id">ID of board game to delete</param>
    /// <returns>Deleted game data</returns>
    [HttpDelete("DeleteGame")]
    [ResponseCache(NoStore = true)]
    public async Task<Dto<BoardGame?>> DeleteGame(int id)
    {
        var game = await _context.BoardGames.FindAsync(id);

        if (game is not null)
        {
            _context.BoardGames.Remove(game);
            await _context.SaveChangesAsync();
        }

        return new Dto<BoardGame?>()
        {
            Data = game,
            Links = [new HttpLink(
                Url.Action(null, "BoardGame", id, Request.Scheme)!, 
                "self", "DELETE")
            ]
        };
    }
}