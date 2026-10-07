using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace WebApplication2.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        
        return View();
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