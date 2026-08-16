using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ass01_FE.Presentation.Models.News; // For NewsArticleDto

namespace ass01_FE.Presentation.Models.Report;

public class ReportRequest
{
    [Required]
    public DateTime? StartDate { get; set; }
    
    [Required]
    public DateTime? EndDate { get; set; }
}

public class ReportStatisticsDto
{
    public int TotalArticles { get; set; }
    public int ActiveArticles { get; set; }
    public int InactiveArticles { get; set; }
    public Dictionary<string, int> ArticlesByCategory { get; set; } = new();
    public Dictionary<string, int> ArticlesByAuthor { get; set; } = new();
    public Dictionary<string, int> ArticlesByStatus { get; set; } = new();
    public List<NewsArticleDto> Articles { get; set; } = new();
}

public class ReportViewModel
{
    public ReportRequest Request { get; set; } = new();
    public ReportStatisticsDto? Statistics { get; set; }
    public string? ErrorMessage { get; set; }
}
