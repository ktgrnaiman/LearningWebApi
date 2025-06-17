using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learning.Models;

[Table("BoardGames_Categories")]
public class BoardGame_Category
{
    [Key]
    public int BoardGameId { get; set; }
    
    [Key]
    public int CategoryId { get; set; }
    
    public BoardGame? BoardGame { get; set; }
    
    public Category? Category { get; set; }
}