using System.ComponentModel.DataAnnotations;

namespace Learning.DTO;

public class BoardGameDto
{
    [Required]
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Year { get; set; }
    
    public int? MinAge { get; set; }
    
    public int? PlayTime { get; set; }
    
    public int? MinPlayers { get; set; }
    
    public int? MaxPlayers { get; set; }
}