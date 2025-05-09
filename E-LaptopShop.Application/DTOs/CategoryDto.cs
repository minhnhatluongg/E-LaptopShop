using System.Text.Json.Serialization;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
public class CategoryCreateDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
public class CategoryUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}