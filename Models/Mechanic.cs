using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learning.Models;

[Table("Mechanics")]
public class Mechanic
{
    [Key]
    public int Id { get; set; }

    [Required] [MaxLength(200)] 
    public string Name { get; set; } = null!;
        
    [Required]
    public DateTime CreatedDate { get; set; }
        
    [Required]
    public DateTime LastModifiedDate { get; set; }
    
    public ICollection<BoardGame_Mechanic>? BoardGamesJunction { get; set; }

    public Mechanic() {}

    public Mechanic(string name, DateTime time)
    {
        Name = name;
        CreatedDate = time;
        LastModifiedDate = time;
    }
}