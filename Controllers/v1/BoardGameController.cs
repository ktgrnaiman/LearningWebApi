using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using Asp.Versioning;
using Learning.Models;

namespace Learning.Controllers.v1
{
    /// <summary>
    /// V1 controller to process CRUD operations on board game entities
    /// </summary>
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion(1.0)]
    public class BoardGameController : ControllerBase
    {
        private readonly ILogger<BoardGameController> _logger;
        
        /// <summary>
        /// Creates object
        /// </summary>
        public BoardGameController(ILogger<BoardGameController> logger) 
        {
            _logger = logger;
        }
        
        /// <summary>
        /// Returns list of all games
        /// </summary>
        [HttpGet("Get")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public IEnumerable<BoardGame> Get() 
        {
            yield return new BoardGame(0, "Axis and Allies", 1981, 8, 16);
            yield return new BoardGame(1, "Citadels", 2000, 3, 10);
            yield return new BoardGame(2, "Terraforming Mars", 2001, 2, 6);
        }
    }
}
