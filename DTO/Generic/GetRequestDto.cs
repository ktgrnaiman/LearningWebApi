using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Learning.Attributes;

namespace Learning.DTO;

//TODO:PageSize and PageIndex do not receive default value.
public class GetRequestDto<T> : IValidatableObject
{
    private readonly SortColumnValidationAttribute _colValidator = new(typeof(T));
    private readonly SortDirectionValidationAttribute _dirValidator = new();
    private readonly FilterQueryValidationAttribute _filterValidator = new();
    
    [DefaultValue(0)]
    public int PageIndex { get; set; }
    
    [DefaultValue(10)]
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

        if (PageIndex < 0)
            result.Add(new ValidationResult("Index of page can't be less than zero"));
        if (PageSize <= 0 || PageSize > 50)
            result.Add(new ValidationResult("Page size should be in (1, 50) range"));
        if (FilterQuery is not null)
            result.Add(_filterValidator.GetValidationResult(FilterQuery, context)!);
        if (SortColumn is not null)
            result.Add(_colValidator.GetValidationResult(SortColumn, context)!);
        if (SortDir is not null)
            result.Add(_dirValidator.GetValidationResult(SortDir, context)!);

        return result.ToArray();
    }
}