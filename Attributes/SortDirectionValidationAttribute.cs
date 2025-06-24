using System.ComponentModel.DataAnnotations;

namespace Learning.Attributes;

public class SortDirectionValidationAttribute : ValidationAttribute
{
    public string[] ValidStrings { get; set; } = ["ASC", "DESC"];

    public SortDirectionValidationAttribute() : base("Value must be on of the following {0}")
    { }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var strValue = value as string;
        if(!string.IsNullOrWhiteSpace(strValue) && (ValidStrings[0] == strValue || ValidStrings[1] == strValue))
            return ValidationResult.Success;
        return new ValidationResult(FormatErrorMessage(string.Join(',', ValidStrings)));
    }
}