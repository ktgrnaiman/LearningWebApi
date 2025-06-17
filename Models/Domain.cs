using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learning.Models;

[Table("Domains")]
public class Domain
{
    [Key]
    public int Id { get; set; }

    [Required] [MaxLength(200)] 
    public string Name { get; set; } = null!;
    
    [Required]
    public DateTime CreatedDate { get; set; }
        
    [Required]
    public DateTime LastModifiedDate { get; set; }
    
    [MaxLength(200)]
    public string Notes { get; set; }
    
    [Required]
    public int Flags { get; set; }
    
    public ICollection<BoardGame_Domain>? BoardGamesJunction { get; set; }
    
    public Publisher? Publisher { get; set; }
}