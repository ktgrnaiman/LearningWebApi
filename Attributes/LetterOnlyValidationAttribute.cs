using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Learning.Attributes;

public partial class LetterOnlyValidationAttribute : ValidationAttribute
{
    [GeneratedRegex(@"^[a-zA-Z]+$")]
    private static partial Regex LetterRegex();
    
    private Func<string, bool> _isValid;
    
    public LetterOnlyValidationAttribute(bool useRegex)
    {
        if (useRegex)
            _isValid = input => LetterRegex().IsMatch(input);
        else
            _isValid = input => input.All(char.IsLetter);
    }
    
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is string strValue)
        {
            if(_isValid(strValue))
               return ValidationResult.Success;
            return new ValidationResult("Name must contain only letters (no spaces, digits, or other chars)");
        }
        return new ValidationResult("This attribute was applied to parameter of type other than string");
    }
}