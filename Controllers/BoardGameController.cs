using Microsoft.AspNetCore.Mvc;
using Learning.DTO;
using Learning.Models;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;

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

    [HttpGet("Get")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    public async Task<Dto<BoardGame[]>> GetGames()
    {
        return new Dto<BoardGame[]>()
        {
            Data = await _context.BoardGames.Take(100).ToArrayAsync(),
            Links = [new HttpLink(
                Url.Action(null, "BoardGame", null, Request.Scheme)!,
                "GET", "self"
            )]
        };
    }
}