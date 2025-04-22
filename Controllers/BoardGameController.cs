using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Learning.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BoardGameController : ControllerBase
    {
        private readonly ILogger<BoardGameController> _logger;

        public BoardGameController(ILogger<BoardGameController> logger) 
        {
            _logger = logger;
        }

        [HttpGet("Get")]
        public IEnumerable<BoardGame> Get() 
        {
            yield return new BoardGame(0, "Axis and Allies", 1981, 8, 16);
            yield return new BoardGame(1, "Citadels", 2000, 3, 10);
            yield return new BoardGame(2, "Terraforming Mars", 2001, 2, 6);
        }
    }
}
