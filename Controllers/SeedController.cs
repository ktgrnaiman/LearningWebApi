using System.Buffers;
using System.Globalization;
using System.Linq.Dynamic.Core;
using CsvHelper;
using CsvHelper.Configuration;
using Learning.Models;
using Learning.Models.CSV;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Learning.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private const string DataLocation = "Data/bgg_dataset.csv";
        
    private IWebHostEnvironment _env;
    private ApplicationDbContext _context;
    private ILogger<SeedController> _logger;
    
    public SeedController(IWebHostEnvironment env, ApplicationDbContext context, ILogger<SeedController> logger)
    {
        _env = env;
        _context = context;
        _logger = logger;
    }

    [HttpPut("Init")]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> Seed(int? count = null)
    {
        var config = new CsvConfiguration(CultureInfo.GetCultureInfo("pt-BR"))
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };

        using var stream = new StreamReader(System.IO.Path.Combine(_env.ContentRootPath, DataLocation));
        using var reader = new CsvReader(stream, config);

        var cachedDomains = new Dictionary<string, Domain>();
        var cachedMechanics = new Dictionary<string, Mechanic>();
        var now = DateTime.UtcNow;
        int skipRows = 0;
        
        var records = reader.GetRecords<BoardGameCsvRecord>();
        if (count.HasValue)
            records = records.Take(count.Value);
        
        foreach (var gameRec in records)
        {
            if (!gameRec.Id.HasValue 
                || String.IsNullOrWhiteSpace(gameRec.Name) 
                || await _context.BoardGames.AnyAsync(gam => gam.Id == gameRec.Id.Value))
            {
                skipRows++;
                continue;
            }

            var game = new BoardGame(gameRec.Id.Value, gameRec.Name)
            {
                BGGRank = gameRec.BGGRank ?? 0,
                ComplexityAverage = gameRec.ComplexityAverage ?? 0,
                MaxPlayers = gameRec.MaxPlayers ?? 0,
                MinPlayers = gameRec.MinPlayers ?? 0,
                MinAge = gameRec.MinAge ?? 0,
                OwnedUsers = gameRec.OwnedUsers ?? 0,
                PlayTime = gameRec.PlayTime ?? 0,
                RatingAverage = gameRec.RatingAverage ?? 0,
                UsersRated = gameRec.UsersRated ?? 0,
                Year = gameRec.YearPublished ?? 0,
                CreatedDate = now,
                LastModifiedDate = now
            };

            _context.BoardGames.Add(game);

            if (!string.IsNullOrWhiteSpace(gameRec.Domains))
                foreach (var domainName in gameRec.Domains
                             .Split(',', StringSplitOptions.TrimEntries)
                             .Distinct(StringComparer.InvariantCultureIgnoreCase))
                {
                    cachedDomains.TryGetValue(domainName, out var domain);
                    domain ??= _context.Domains.FirstOrDefault(d => d.Name == domainName);
                        
                    if (domain is null)
                    {
                        domain = new Domain(domainName, now);
                        cachedDomains.Add(domainName, domain);
                        _context.Domains.Add(domain);
                    }
                    _context.GameDomainJunctions.Add(new BoardGame_Domain(game, domain, now));
                }
            
            if(!string.IsNullOrWhiteSpace(gameRec.Mechanics))
                foreach (var mechName in gameRec.Mechanics
                             .Split(',', StringSplitOptions.TrimEntries)
                             .Distinct(StringComparer.InvariantCultureIgnoreCase))
                {
                    cachedMechanics.TryGetValue(mechName, out var mechanic);
                    mechanic ??= _context.Mechanics.FirstOrDefault(m => m.Name == mechName);
                        
                    if (mechanic is null)
                    {
                        mechanic = new Mechanic(mechName, now);
                        cachedMechanics.Add(mechName, mechanic);
                        _context.Mechanics.Add(mechanic);
                    }
                    _context.GameMechanicJunctions.Add(new BoardGame_Mechanic(game, mechanic, now));
                }
        }

        await _context.SaveChangesAsync();
        
        return new JsonResult(new
        {
            BoardGames = _context.BoardGames.Count(),
            Domain = _context.Domains.Count(),
            Mechanic = _context.Mechanics.Count(),
            SkippedRows = skipRows
        });
    }
}