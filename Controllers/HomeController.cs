using partywebapp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using partywebapp.Services;
namespace partywebapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PartiesService _partiesService;

        public HomeController(ILogger<HomeController> logger, PartiesService service)
        {
            _logger = logger;
            _partiesService = service;
        }

        //[Authorize]
        public IActionResult Index()
        {

            HomePage homePage = new HomePage();
            return View(_partiesService.getDataForHomePage(homePage));
        }

        [Authorize(Roles = "Admin")] //only admin can see this 
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
