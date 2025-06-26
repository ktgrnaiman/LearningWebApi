using System.ComponentModel.DataAnnotations;
using Learning.Attributes;

namespace Learning.DTO;

public class MechanicDto
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    [LetterOnlyValidation(useRegex: false)]
    public string Name { get; set; } = null!;
}