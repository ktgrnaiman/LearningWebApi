using System.ComponentModel.DataAnnotations;

namespace Learning.DTO;

public class MechanicDto
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = null!;
}