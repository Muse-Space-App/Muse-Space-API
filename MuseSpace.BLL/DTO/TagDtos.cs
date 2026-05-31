namespace MuseSpace.BLL.DTO;

public class TagResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}

public class PopularTagResponse : TagResponse
{
    public int UsageCount { get; set; }
}
