using System.ComponentModel.DataAnnotations;

namespace Learning.Attributes;

public class FilterQueryValidationAttribute : ValidationAttribute
{
    public FilterQueryValidationAttribute() : base("Filter query can't be empty")
    { }
    
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var strValue = value as string;
        if (!string.IsNullOrWhiteSpace(strValue))
            return ValidationResult.Success;
        return new ValidationResult(ErrorMessage);
    }
}