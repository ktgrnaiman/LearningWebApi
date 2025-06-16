using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learning.Models;

[Table("BoardGames_Domains")]
public class BoardGame_Domain
{
    [Key]
    public int BoardGameId { get; set; }
    
    [Key]
    public int DomainId { get; set; }
    
    [Required]
    public DateTime CreatedDate { get; set; }
    
    public BoardGame? BoardGame { get; set; }
    
    public Domain? Domain { get; set; }
}