using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learning.Models;

[Table("BoardGames_Mechanics")]
public class BoardGame_Mechanic
{
    [Key]
    public int BoardGameId { get; set; }
    
    [Key]
    public int MechanicId { get; set; }
    
    [Required]
    public DateTime CreatedDate { get; set; }
    
    [Required]
    public BoardGame? BoardGame { get; set; }
    
    [Required]
    public Mechanic? Mechanic { get; set; }
    
    public BoardGame_Mechanic() {}
    
    public BoardGame_Mechanic(BoardGame game, Mechanic mechanic, DateTime time)
    {
        BoardGame = game;
        Mechanic = mechanic;
        CreatedDate = time;
    }
}