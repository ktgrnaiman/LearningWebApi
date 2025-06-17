using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learning.Models;

[Table("Categories")]
public class Category
{
    [Key]
    public int Id { get; set; }
    
    [Required] [MaxLength(200)]
    public string Name { get; set; }
    
    [Required]
    public DateTime CreatedDate { get; set; }
    
    [Required]
    public DateTime LastModifiedDate { get; set; }
    
    public ICollection<BoardGame_Category>? BoardGamesJunction { get; set; }
}