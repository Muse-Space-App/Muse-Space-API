namespace MuseSpace.BLL.DTO;

public class AdvancedSearchRequest
{
    public string? Query { get; set; }
    public string? ExactTag { get; set; }
    public string SortBy { get; set; } = "newest"; // newest, popular, views
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
