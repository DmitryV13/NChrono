using FeaneMVC.Models;
using FinalProject.DbModel;
using Microsoft.AspNetCore.Mvc;

namespace FeaneMVC.Controllers;

[ApiController]
[Route("api/filters")]
public class FiltersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly WebApplication1.Interfaces.ISession _sessionService;
    
    public FiltersController(ApplicationDbContext context, WebApplication1.Interfaces.ISession sessionService)
    {
        _context = context;
        _sessionService = sessionService;
    }

    [HttpPost]
    public IActionResult AddFilters([FromBody] List<string> filters)
    {
        if (filters == null || filters.Count == 0)
        {
            return BadRequest("No filters provided.");
        }
        
        var userId = _sessionService.GetUserId();

        foreach (var filter in filters)
        {
            var userFilter = new UserFilter
            {
                Id = Guid.NewGuid(),
                Filter = filter,
                UserId = userId
            };
            _context.UserFilters.Add(userFilter);
        }

        _context.SaveChanges();

        return Ok(new { Message = "Filters saved.", Filters = filters });
    }

    public IActionResult Index()
    {
        return View();
    }
    [HttpGet("filter-page")]
    public IActionResult FilterPage()
    {
        var userId = _sessionService.GetUserId();

        var userFilters = _context.UserFilters
            .Where(f => f.UserId == userId)
            .Select(f => f.Filter)
            .Distinct()
            .ToList();
        
        
        UserFilterNotificationList filters = new UserFilterNotificationList();
        filters.Filter = userFilters;
        filters.Id = userId;
        foreach (var filter in userFilters)
        {
            filters.maDictionary.Add(filter, new List<string>(){"sdfsdfdsf","sdfdsfsdfsdf","sdfsdfsdf"});
        }
        ViewData["Title"] = "Filters";
        return View(filters);
    }
    
    [HttpGet("all")]
    public IActionResult GetUserFilters()
    {
        var userId = _sessionService.GetUserId();

        var userFilters = _context.UserFilters
            .Where(f => f.UserId == userId)
            .Select(f => f.Filter)
            .ToList();
        
        
        Dictionary<string, List<string>> filters = new Dictionary<string, List<string>>();
        foreach (var filter in userFilters)
        {
            filters.Add(filter, new List<string>(){"sdfsdfdsf","sdfdsfsdfsdf","sdfsdfsdf"});
        }
        return Ok(filters);
    }
}