using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioAnalyticsApi.Data;
using PortfolioAnalyticsApi.Models;
using System.Text.Json;

namespace PortfolioAnalyticsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly AnalyticsDbContext _context;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(AnalyticsDbContext context, ILogger<AnalyticsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("track")]
    public async Task<IActionResult> TrackEvent([FromBody] TrackEventRequest request)
    {
        try
        {
            var analyticsEvent = new AnalyticsEvent
            {
                EventType = request.EventType,
                PageUrl = request.PageUrl,
                PageTitle = request.PageTitle,
                Referrer = request.Referrer,
                UtmSource = request.UtmSource,
                UtmMedium = request.UtmMedium,
                UtmCampaign = request.UtmCampaign,
                SessionId = request.SessionId ?? Guid.NewGuid(),
                IpAddress = GetClientIp(),
                UserAgent = Request.Headers["User-Agent"].ToString(),
                Browser = request.Browser,
                Os = request.Os,
                Device = request.Device,
                ScreenResolution = request.ScreenResolution,
                ViewportWidth = request.ViewportWidth,
                ViewportHeight = request.ViewportHeight,
                LoadTime = request.LoadTime,
                CustomData = request.CustomData != null ? JsonSerializer.Serialize(request.CustomData) : null
            };

            _context.AnalyticsEvents.Add(analyticsEvent);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, eventId = analyticsEvent.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking event");
            return StatusCode(500, new { success = false, error = "Internal server error" });
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.AddDays(-7);
        var toDate = to ?? DateTime.UtcNow;

        var stats = await _context.AnalyticsEvents
            .Where(e => e.Timestamp >= fromDate && e.Timestamp <= toDate)
            .GroupBy(e => 1)
            .Select(g => new
            {
                totalEvents = g.Count(),
                uniqueSessions = g.Select(e => e.SessionId).Distinct().Count(),
                pageViews = g.Count(e => e.EventType == "pageview"),
                topPages = g.Where(e => e.EventType == "pageview")
                    .GroupBy(e => e.PageUrl)
                    .OrderByDescending(pg => pg.Count())
                    .Take(10)
                    .Select(pg => new { url = pg.Key, count = pg.Count() })
                    .ToList(),
                topReferrers = g.Where(e => !string.IsNullOrEmpty(e.Referrer))
                    .GroupBy(e => e.Referrer)
                    .OrderByDescending(r => r.Count())
                    .Take(10)
                    .Select(r => new { referrer = r.Key, count = r.Count() })
                    .ToList(),
                deviceBreakdown = g.GroupBy(e => e.Device)
                    .Select(d => new { device = d.Key, count = d.Count() })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        return Ok(stats);
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    private string? GetClientIp()
    {
        return Request.Headers["X-Forwarded-For"].FirstOrDefault() 
            ?? Request.HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
