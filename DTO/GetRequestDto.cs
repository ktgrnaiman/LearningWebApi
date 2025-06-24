using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Learning.Attributes;

namespace Learning.DTO;

public class GetRequestDto<T> : IValidatableObject
{
    private readonly SortColumnValidationAttribute _colValidator = new(typeof(T));
    private readonly SortDirectionValidationAttribute _dirValidator = new();
    
    [DefaultValue(0)]
    public int PageIndex { get; set; }
    
    [DefaultValue(10)]
    [Range(1, 50)]
    public int PageSize { get; set; }

    [DefaultValue(null)]
    public string? FilterQuery { get; set; }
    
    [DefaultValue(null)]
    public string? SortColumn { get; set; }
    
    [DefaultValue("ASC")]
    public string? SortDir { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        var result = new List<ValidationResult>();

        if (SortColumn is not null)
            result.Add(_colValidator.GetValidationResult(SortColumn, context));
        if (SortDir is not null)
            result.Add(_dirValidator.GetValidationResult(SortDir, context));

        return result.ToArray();
    }
}