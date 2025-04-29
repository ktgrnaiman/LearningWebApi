using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using Asp.Versioning;
using Learning.DTO;
using Learning.Models;
using Microsoft.AspNetCore.Cors;

namespace Learning.Controllers.v2
{
    /// <summary>
    /// V2 controller to process CRUD operations on board game entities
    /// </summary>
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion(2.0)]
    public class BoardGameController : ControllerBase
    {
        private readonly ILogger<BoardGameController> _logger;
        private readonly IEnumerable<HttpLink> _links;
        
        public BoardGameController(ILogger<BoardGameController> logger) 
        {
            _logger = logger;
        }
        
        /// <summary>
        /// Returns DTO list of all games
        /// </summary>
        [HttpGet("Get")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public Dto<IEnumerable<BoardGame>> Get()
        {
            return new Dto<IEnumerable<BoardGame>>()
            {
                Data = GetData(),
                Links = [new HttpLink(
                    Url.Action(null, "BoardGame", null, Request.Scheme)!,
                    "GET", "self"
                )]
            };
        }

        private IEnumerable<BoardGame> GetData()
        {
            yield return new BoardGame(0, "Axis and Allies", 1981, 8, 16);
            yield return new BoardGame(1, "Citadels", 2000, 3, 10);
            yield return new BoardGame(2, "Terraforming Mars", 2001, 2, 6);
        }
    }
}
