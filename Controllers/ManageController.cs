using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using partywebapp.Services;
using partywebapp.Models;
using System.Security.Claims;
using partywebapp.Data;
using Microsoft.AspNetCore.Authorization;

namespace partywebapp.Controllers
{
    public class ManageController : Controller
    {

        private readonly IManageService _manageService;
        private readonly PartyWebAppContext _context;

        public ManageController(IManageService manageService, PartyWebAppContext context)
        {
            _manageService = manageService;
            _context = context;
        }

        public int findCurrentUserId()
        {
            return Int32.Parse(HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        public void initTypeUserToViewData(User currentUser)
        {
            ViewData["UserFullName"] = currentUser.firstName + " " + currentUser.lastName;
            if (currentUser.Type == UserType.Admin)
            {
                ViewData["UserType"] = "Manager";
            }
            else if (currentUser.Type == UserType.producer)
            {
                ViewData["UserType"] = "Producer";
            }
            else
            {
                ViewData["UserType"] = "Client";
            }
        }
        public User returnCurrentUser()
        {
            var currentUser = _context.User.FirstOrDefault(u => u.Id == findCurrentUserId());
            initTypeUserToViewData(currentUser);
            return currentUser;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            initTypeUserToViewData(returnCurrentUser());
            return View();
        }
        [Authorize(Roles = "Admin")]
        public string GenreStatistics()
        {
            return _manageService.GetPartiesInGenre();
        }
        [Authorize(Roles = "Admin")]
        public string ClubStatistics()
        {
            return _manageService.GetPartiesInClub();
        }
        [Authorize(Roles = "Admin")]
        public string AreaStatistics()
        {
            return _manageService.GetPartiesInArea();
        }
    }
}
