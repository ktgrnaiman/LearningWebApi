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
    
    [Required]
    public BoardGame? BoardGame { get; set; }
    
    [Required]
    public Domain? Domain { get; set; }
    
    public BoardGame_Domain(){}
    
    public BoardGame_Domain(BoardGame game, Domain domain, DateTime time)
    {
        BoardGame = game;
        Domain = domain;
        CreatedDate = time;
    }
}