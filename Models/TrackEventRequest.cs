using System.ComponentModel.DataAnnotations;

namespace PortfolioAnalyticsApi.Models;

public class TrackEventRequest
{
    [Required]
    public string EventType { get; set; } = string.Empty;
    
    public string? PageUrl { get; set; }
    public string? PageTitle { get; set; }
    public string? Referrer { get; set; }
    public string? UtmSource { get; set; }
    public string? UtmMedium { get; set; }
    public string? UtmCampaign { get; set; }
    public Guid? SessionId { get; set; }
    public string? Browser { get; set; }
    public string? Os { get; set; }
    public string? Device { get; set; }
    public string? ScreenResolution { get; set; }
    public int? ViewportWidth { get; set; }
    public int? ViewportHeight { get; set; }
    public int? LoadTime { get; set; }
    public Dictionary<string, object>? CustomData { get; set; }
}
