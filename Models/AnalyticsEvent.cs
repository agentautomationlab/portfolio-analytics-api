using System.ComponentModel.DataAnnotations;

namespace PortfolioAnalyticsApi.Models;

public class AnalyticsEvent
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [Required]
    [MaxLength(100)]
    public string EventType { get; set; } = string.Empty; // pageview, click, scroll, etc.
    
    [MaxLength(2000)]
    public string? PageUrl { get; set; }
    
    [MaxLength(500)]
    public string? PageTitle { get; set; }
    
    [MaxLength(2000)]
    public string? Referrer { get; set; }
    
    [MaxLength(500)]
    public string? UtmSource { get; set; }
    
    [MaxLength(500)]
    public string? UtmMedium { get; set; }
    
    [MaxLength(500)]
    public string? UtmCampaign { get; set; }
    
    public Guid? SessionId { get; set; }
    
    [MaxLength(50)]
    public string? IpAddress { get; set; }
    
    [MaxLength(100)]
    public string? Country { get; set; }
    
    [MaxLength(100)]
    public string? City { get; set; }
    
    [MaxLength(500)]
    public string? UserAgent { get; set; }
    
    [MaxLength(100)]
    public string? Browser { get; set; }
    
    [MaxLength(100)]
    public string? Os { get; set; }
    
    [MaxLength(50)]
    public string? Device { get; set; } // mobile, desktop, tablet
    
    [MaxLength(20)]
    public string? ScreenResolution { get; set; }
    
    public int? ViewportWidth { get; set; }
    
    public int? ViewportHeight { get; set; }
    
    public int? LoadTime { get; set; } // Page load time in ms
    
    public string? CustomData { get; set; } // JSON string for additional data
}
