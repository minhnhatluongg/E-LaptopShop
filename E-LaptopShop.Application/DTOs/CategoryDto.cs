using System.Text.Json.Serialization;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Slug { get; set; } = null!;
}
public class CategoryCreateRequestDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
public class CategoryUpdateRequestDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Slug { get; set; } = null!;
}

