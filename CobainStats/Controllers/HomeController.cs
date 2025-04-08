using System.Diagnostics;
using CobainStats.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CobainStats.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly StatsContext _context;

        public HomeController(ILogger<HomeController> logger, StatsContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await _context
            .Set<DailyStat>()
            .FromSqlRaw(@"
                SELECT
                    DATE(timestamp) AS Date,
                    COUNT(DISTINCT chat_id) AS DailyActiveUsers
                FROM links
                WHERE timestamp >= NOW() - INTERVAL '30 days'
                GROUP BY DATE(timestamp)
                ORDER BY Date DESC
            ")
            .ToListAsync();

            return View(stats);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
