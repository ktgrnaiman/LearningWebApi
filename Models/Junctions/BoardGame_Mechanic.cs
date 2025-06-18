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
    
    public BoardGame? BoardGame { get; set; }
    
    public Mechanic? Mechanic { get; set; }
}