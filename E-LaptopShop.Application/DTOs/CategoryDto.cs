using System.Text.Json.Serialization;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Slug { get; set; } = null!;
}
public class CategoryCreateDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    [JsonIgnore]
    public string Slug { get; set; } = null!;

}
public class CategoryUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Slug { get; set; } = null!;

}

