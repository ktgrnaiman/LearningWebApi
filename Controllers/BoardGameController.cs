using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Learning.DTO;
using Learning.Models;
using Learning.Attributes;
using Learning.Constants;

namespace Learning.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardGameController(ApplicationDbContext context, ILogger<BoardGameController> logger)
    : ControllerBase
{
    private readonly ILogger<BoardGameController> _logger = logger;
    private readonly ApplicationDbContext _context = context;

    /// <summary>
    /// Returns paginated entries in DTO
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Array of board games in DTO</returns>
    [HttpGet("GetGame")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    [ManualValidationFilter]
    public async Task<ActionResult<ResponseDto<BoardGame[]>>> GetGames([FromQuery]GetRequestDto<BoardGameDto> request)
    {
        var validationResult = HandleGetModelState();
        if (validationResult is not null)
            return validationResult;
        
        _logger.LogInformation(CustomLogEvents.BoardGameGet, "Get method started at ");
        
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
        
        return new ResponseDto<BoardGame[]>()
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

    private ActionResult? HandleGetModelState()
    {
        if (!ModelState.IsValid)
        {
            var details = new ValidationProblemDetails(ModelState);
            details.Extensions["traceId"] = System.Diagnostics.Activity.Current?.Id ??
                                            HttpContext.TraceIdentifier;
            if (ModelState.Keys.Any(k => k == "PageSize"))
            {
                details.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.2";
                details.Status = StatusCodes.Status501NotImplemented;
                return new ObjectResult(details) {
                    StatusCode = StatusCodes.Status501NotImplemented
                };
            }
            else
            {
                details.Type = "https:/ /tools.ietf.org/html/rfc7231#section-6.5.1";
                details.Status = StatusCodes.Status400BadRequest;
                return new BadRequestObjectResult(details);
            }
        }
        return null;
    }
    
    /// <summary>
    /// Updates board game's name, year properties
    /// </summary>
    /// <param name="update">New values for updating</param>
    /// <returns>Updated board game entry</returns>
    [HttpPost("UpdateGame")]
    [ResponseCache(NoStore = true)]
    public async Task<ResponseDto<BoardGame?>> PostGame(BoardGameDto update)
    {
        var game = await _context.BoardGames.FindAsync(update.Id);

        if (game is not null)
        {
            if (!string.IsNullOrWhiteSpace(update.Name))
                game.Name = update.Name;
            if (update.Year is > 0)
                game.Year = update.Year.Value;
            game.LastModifiedDate = DateTime.UtcNow;
            _context.BoardGames.Update(game);
            await _context.SaveChangesAsync();
        }

        return new ResponseDto<BoardGame?>()
        {
            Data = game,
            Links = [new HttpLink(
                Url.Action(null, "BoardGame", update, Request.Scheme)!, 
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
    public async Task<ResponseDto<BoardGame?>> DeleteGame(int id)
    {
        var game = await _context.BoardGames.FindAsync(id);

        if (game is not null)
        {
            _context.BoardGames.Remove(game);
            await _context.SaveChangesAsync();
        }

        return new ResponseDto<BoardGame?>()
        {
            Data = game,
            Links = [new HttpLink(
                Url.Action(null, "BoardGame", id, Request.Scheme)!, 
                "self", "DELETE")
            ]
        };
    }
}