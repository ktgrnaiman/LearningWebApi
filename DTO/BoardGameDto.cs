using System.ComponentModel.DataAnnotations;

namespace Learning.DTO;

public class BoardGameDto: IValidatableObject
{
    [Required]
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Year { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Name is null && Year is null)
            return [new ValidationResult("You passed empty data to update")];
        return Array.Empty<ValidationResult>();
    }
}