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
    /// <param name="filterQuery">Query string with which names should start</param>
    /// <param name="sortColumn">Name of column used as criteria for sorting</param>
    /// <param name="sortAsc">Ascending or descending sorting</param>
    /// <returns>Array of board games in DTO</returns>
    [HttpGet("GetGame")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    public async Task<Dto<BoardGame[]>> GetGames(
        int pageIndex = 0, int pageSize = 10, 
        string? filterQuery = null, 
        string? sortColumn = null, bool sortAsc = true)
    {
        IQueryable<BoardGame> query = _context.BoardGames;

        if (!string.IsNullOrWhiteSpace(filterQuery))
            query = query.Where(game => game.Name.StartsWith(filterQuery));
        int entryCount = await query.CountAsync();
        
        if (!string.IsNullOrWhiteSpace(sortColumn))
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
                "self", "GET"
            )]
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
            if (updateGame.MinAge is >= 0)
                game.MinAge = updateGame.MinAge.Value;
            if (updateGame.PlayTime is > 0)
                game.PlayTime = updateGame.PlayTime.Value;
            if (updateGame.MinPlayers is > 0)
                game.MinPlayers = updateGame.MinPlayers.Value;
            if (updateGame.MaxPlayers is > 0)
                game.MaxPlayers = updateGame.MaxPlayers.Value;
            
            game.LastModifiedDate = DateTime.UtcNow;
            _context.Update(game);
            await _context.SaveChangesAsync();
        }

        return new Dto<BoardGame?>()
        {
            Data = game,
            Links = [new HttpLink(
                Url.Action(null, "BoardGame", updateGame, Request.Scheme)!, 
                "self", "POST"
            )]
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
                "self", "DELETE"
            )]
        };
    }
    
    /// <summary>
    /// Deletes board games entry with specified unique IDs
    /// </summary>
    /// <param name="ids">IDs of board game to delete</param>
    /// <returns>Deleted games data</returns>
    [HttpDelete("DeleteGames")]
    [ResponseCache(NoStore = true)]
    public async Task<Dto<BoardGame[]?>> DeleteGames(int[] ids)
    {
        var games = new List<BoardGame>(ids.Length);
        
        foreach (int id in ids)
        {
            var game = await _context.BoardGames.FindAsync(id);
            if (game is not null)
            {
                games.Add(game);
                _context.BoardGames.Remove(game);
            }
        }
        
        await _context.SaveChangesAsync();
        
        return new Dto<BoardGame[]?>()
        {
            Data = games.ToArray(),
            Links = [new HttpLink(
                Url.Action(null, "BoardGame", ids, Request.Scheme)!, 
                "self", "DELETE"
            )]
        };
    }
}