using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Learning.Attributes;

public class SortColumnValidationAttribute : ValidationAttribute
{
    public Type EntityType { get; set; }
    public IEnumerable<PropertyInfo> EntityProperties { get; set; }
    
    public SortColumnValidationAttribute(Type entityType) : base("Specified value doesn't match entity's property names")
    {
        EntityType = entityType ?? throw new NullReferenceException("entityType ctor parameter can't be null");
        EntityProperties = EntityType.GetProperties();
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var strValue = value as string;
        if(!string.IsNullOrWhiteSpace(strValue))
            if (EntityProperties.Any(p => p.Name == strValue))
                return ValidationResult.Success;
            
        return new ValidationResult(ErrorMessage);
    }
}