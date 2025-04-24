using System.ComponentModel.DataAnnotations;

namespace Drugs.Library.Models;

public record Category
{
    public int CategoryId { get; init; }
    public string Name { get; init; }
    // [Range((int)ConsoleColor.Black, (int)ConsoleColor.White, ErrorMessage = "Color code must be a valid ConsoleColor.")]
    // public int? ColorCode { get; init; }
    public ConsoleColor? ColorCode { get; init; } 

    public Category()
    {
    }

    public Category(int categoryId, string name, ConsoleColor? colorCode)
    {
        CategoryId = categoryId;
        Name = name;
        ColorCode = colorCode;
    }
}