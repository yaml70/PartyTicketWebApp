using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using partywebapp.Data;
using partywebapp.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using partywebapp.Services;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace partywebapp.Controllers
{
    public class UsersController : Controller
    {
        private readonly PartyWebAppContext _context;
        private readonly PartiesService _partiesService;
        
        public UsersController(PartyWebAppContext context, PartiesService service)
        {
            _context = context;
            _partiesService = service;
        }
        
        public int findCurrentUserId()
        {
            return Int32.Parse(HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        
        public User returnCurrentUser()
        {
            var currentUser = _context.User.FirstOrDefault(u => u.Id == findCurrentUserId());
            initTypeUserToViewData(currentUser);
            return currentUser;
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
        
        public void userTypesViewList()
        {
            List<SelectListItem> TypesUser = new List<SelectListItem>();
            TypesUser.Add(new SelectListItem() { Text = "Client", Value = "0" });
            TypesUser.Add(new SelectListItem() { Text = "Producer", Value = "1" });
            TypesUser.Add(new SelectListItem() { Text = "Admin", Value = "2" });
            ViewData["TypesUser"] = TypesUser;
        }
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
