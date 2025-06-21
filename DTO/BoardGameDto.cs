using System.ComponentModel.DataAnnotations;

namespace Learning.DTO;

public class BoardGameDto(int id, string? name, int? year)
{
    [Required]
    public int Id { get; set; } = id;

    public string? Name { get; set; } = name;

    public int? Year { get; set; } = year;
}