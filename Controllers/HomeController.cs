using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Trion.Models;

namespace Trion.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        // private readonly RoleManager<IdentityRole> _rm;
        //RoleManager<IdentityRole> rm
        public HomeController(ILogger<HomeController> logger)
        {   //_rm = rm;
            _logger = logger;
        }
       
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
}
