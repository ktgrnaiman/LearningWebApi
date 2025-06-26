using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using System.Text.RegularExpressions;

namespace Learning.DTO;

public class DomainDto : IValidatableObject
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only letters (no spaces, digits, or other chars)")]
    public string Name { get; set; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Id != 3 && Name != "Wargames")
            return [new ValidationResult("Id and/or Name values must match an allowed Domain")];
        return [ValidationResult.Success!];
    }
}