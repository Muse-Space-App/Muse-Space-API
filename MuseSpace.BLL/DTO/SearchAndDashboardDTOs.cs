namespace MuseSpace.BLL.DTO;

public class SearchResponse
{
    public IReadOnlyCollection<ArtworkResponse> Artworks { get; set; } = new List<ArtworkResponse>();
    public IReadOnlyCollection<UserProfileResponse> Users { get; set; } = new List<UserProfileResponse>();
    public IReadOnlyCollection<TagResponse> Tags { get; set; } = new List<TagResponse>();
}

public class DashboardStatsResponse
{
    public int TotalViews { get; set; }
    public int TotalLikes { get; set; }
    public int TotalComments { get; set; }
    public int TotalFollowers { get; set; }

    // Commission Stats
    public int PendingCommissions { get; set; }
    public int ActiveCommissions { get; set; }
    public int CompletedCommissions { get; set; }
    public decimal TotalRevenue { get; set; }
}
