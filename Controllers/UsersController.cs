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
        
                public async Task<IActionResult> Index()
        {
            var currentUser = returnCurrentUser();
            ViewData["userId"] = currentUser.Id;
            userTypesViewList();

            initTypeUserToViewData(currentUser);
            var result = (from o in _context.Party
                          group o by o.areaId into o
                          orderby o.Sum(c => c.areaId) descending
                          select new { o.Key, Total = o.Sum(c => c.ticketsPurchased) }).FirstOrDefault();

            var mostPopularArea = _context.Area.Find(result.Key);

            if (mostPopularArea != null)
            {
                ViewData["MostPopularArea"] = _partiesService.areaTypeToString(mostPopularArea.Type);
            }

            var partyWebAppContext = _context.User.Include(u => u.parties);
            return View(await partyWebAppContext.ToListAsync());
        }

        const int centerId = 1;
        const int northId = 2;
        const int southId = 3;
        const int hasharonId = 4;

        public Task<IActionResult> areaCenterId()
        {
            return AreaPage(centerId);
        }
        public Task<IActionResult> areaNorthId()
        {
            return AreaPage(northId);
        }
        public Task<IActionResult> areaSouthId()
        {
            return AreaPage(southId);
        }
        public Task<IActionResult> areaHasharonId()
        {
            return AreaPage(hasharonId);
        }

        public async Task<IActionResult> AreaPage(int areaId)
        {
            initTypeUserToViewData(returnCurrentUser());
            var q = from a in _context.Area
                    join p in _context.Party on
                    a.Id equals p.areaId
                    where a.Id == areaId
                    select a;

            Area area = q.Include(a => a.Parties).FirstOrDefault();
            List<Party> parties = new List<Party>();
            if (area != null)
            {
                await _context.SaveChangesAsync();
                parties = area.Parties;
                ViewData["AreaName"] = _partiesService.areaTypeToString(area.Type);
            }
            List<PartyImage> images = new List<PartyImage>();
            foreach (Party p in parties)
            {
                PartyImage image = (_context.PartyImage.Where(i => i.PartyId.Equals(p.Id)).FirstOrDefault());
                images.Add(image);
            }
            ViewData["images"] = images;
            Tuple<List<Party>, List<PartyImage>> tuple = new Tuple<List<Party>, List<PartyImage>>(parties, images);
            return View(nameof(AreaPage), tuple);
        }

        public void MostPopularArea()
        {
            var result = (from o in _context.Party
                          group o by o.areaId into o
                          orderby o.Sum(c => c.areaId) descending
                          select new { o.Key, Total = o.Sum(c => c.areaId) }).FirstOrDefault();

            var mostPopularArea = _context.Area.Find(result.Key);

            if (mostPopularArea != null)
            {
                ViewData["MostPopularArea"] = _partiesService.areaTypeToString(mostPopularArea.Type);
                ViewData["CountOfPartiesInMostPopularArea"] = mostPopularArea.Parties.Count();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private async void Signin(User account)
        {
            var claims = new List<Claim>
                {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.email),
                new Claim(ClaimTypes.Role, account.Type.ToString())
                };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
        }

        // GET: Users/Login
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult chooseRegisterType()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Id,email,password")] User user)
        {
            var foundUser = from u in _context.User
                            where u.email == user.email && u.password == user.password
                            select u;

            if (foundUser.Count() != 0)
            {
                await _context.SaveChangesAsync();
                var connectedUser = _context.User.FirstOrDefault(u => u.email == user.email && u.password == user.password);
                Signin(connectedUser);
                ViewData["Full Name"] = connectedUser.firstName + " " + connectedUser.lastName;
                return RedirectToAction("Index", "Parties");
            }
            else
            {
                ViewData["Error"] = "Username and/or password are incorrect.";
            }
            return View(user);
        }

        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,firstName,lastName,email,birthDate,password")] User user)
        {
            if (ModelState.IsValid)
            {
                var q = _context.User.FirstOrDefault(u => u.email == user.email); //if we dont have the email in the DB - return null

                if (q == null)
                {
                    _context.Add(user);
                    await _context.SaveChangesAsync();

                    var u = _context.User.FirstOrDefault(u => u.email == user.email && u.password == user.password);
                    Signin(u);

                    return RedirectToAction("Index", "Parties");
                }
                else
                {

                    ViewData["Error"] = "Unable to comply; cannot register this user.";

                }
            }
            return View(user);
        }
        public IActionResult producerRegister()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> producerRegister([Bind("Id,firstName,lastName,email,birthDate,password")] User user)
        {
            if (ModelState.IsValid)
            {
                var q = _context.User.FirstOrDefault(u => u.email == user.email);

                if (q == null)
                {
                    user.Type = UserType.producer;
                    _context.Add(user);
                    await _context.SaveChangesAsync();

                    var u = _context.User.FirstOrDefault(u => u.email == user.email && u.password == user.password);
                    Signin(u);

                    return RedirectToAction("Index", "Parties");
                }
                else
                {

                    ViewData["Error"] = "Unable to comply; cannot register this user.";

                }
            }
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (returnCurrentUser().Id == id)
            {
                return View("AccessDenied");
            }
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.Include(u => u.parties)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            userTypesViewList();
            initTypeUserToViewData(returnCurrentUser());

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                if (user.Type == UserType.Admin)
                {
                   return View("AccessDenied");
                }
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,firstName,lastName,password,email,birthDate,Type")] User user)
        {
            initTypeUserToViewData(returnCurrentUser());
            if (id != user.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            if(user.Type == UserType.Admin)
            {
                return View("AccessDenied");
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
        
    }
}
