using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using partywebapp.Data;
using partywebapp.Models;
using partywebapp.Services;

namespace partywebapp.Controllers
{
    public class PartiesController : Controller
    {
        private readonly PartyWebAppContext _context;
        private readonly PartiesService _partiesService;
        private readonly ISpotifyClientService _spotifyClientService;
        
        public PartiesController(PartyWebAppContext context, PartiesService service, ISpotifyClientService spotifyClientService)
        {
            _context = context;
            _partiesService = service;
            _spotifyClientService = spotifyClientService;
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
        // GET: Parties
        [Authorize]
        public async Task<IActionResult> Index()
        {
            ViewData["PageName"] = "Index";
            initTypeUserToViewData(returnCurrentUser());
            var partyWebAppContext = _context.Party.Include(p => p.area)
                .Include(p => p.club)
                .Include(p => p.genre)
                .Include(p => p.partyImage)
                .Include(p => p.performers);

            return View(await partyWebAppContext.ToListAsync());
        }

        [Authorize]
        public async Task<IActionResult> homePage()
        {
            ViewData["genreId"] = new SelectList(_context.Set<Genre>(), "Id", "Type");
            ViewData["clubId"] = new SelectList(_context.Set<Club>(), "Id", "Name");
            ViewData["areaId"] = new SelectList(_context.Set<Area>(), "Id", "Type");

            ViewData["PageName"] = "All Parties";
            initTypeUserToViewData(returnCurrentUser());
            var partyWebAppContext = _context.Party.Include(p => p.area)
                        .Include(p => p.club)
                        .Include(p => p.genre)
                        .Include(p => p.partyImage)
                        .Include(p => p.performers);

            return View(await partyWebAppContext.ToListAsync());
        }

        [HttpPost]
        [Authorize]
        public ActionResult findParty(string partyName)
        {

            return RedirectToAction(nameof(findPartyResult), new { partyName = partyName });

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> findPartyResult(string partyName)
        {
            ViewData["genreId"] = new SelectList(_context.Set<Genre>(), "Id", "Type");
            ViewData["clubId"] = new SelectList(_context.Set<Club>(), "Id", "Name");
            ViewData["areaId"] = new SelectList(_context.Set<Area>(), "Id", "Type");

            initTypeUserToViewData(returnCurrentUser());
            var partyWebAppContext = _context.Party.Include(p => p.area)
                .Include(p => p.club)
                .Include(p => p.genre)
                .Include(p => p.partyImage)
                .Include(p => p.performers);
            if (!String.IsNullOrEmpty(partyName))
            {
                var reslut = partyWebAppContext.Where(p => p.name.Replace(" ", "").Contains(partyName.Replace(" ", "")));
                return View("HomePage", await reslut.ToListAsync());

            }
            else
            {
                var reslut = partyWebAppContext.Where(p => p.name.Contains(partyName));
                return View("HomePage", await reslut.ToListAsync());
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult SearchByGenreClubArea(int? genreId, int? clubId, int? areaId)
        {
            return RedirectToAction(nameof(SearchByGenreClubAreaResult), new { genreId = genreId, clubId = clubId, areaId = areaId });
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SearchByGenreClubAreaResult(int? genreId, int? clubId, int? areaId)
        {
            initTypeUserToViewData(returnCurrentUser());
            var PartyWebAppContext = _context.Party.Include(p => p.genre)
                .Include(p => p.club).Include(p => p.area).Include(p => p.partyImage)
                .Where(p => p.genreId.Equals(genreId) &&
                p.clubId.Equals(clubId) && p.areaId.Equals(areaId));

            ViewData["genreId"] = new SelectList(_context.Set<Genre>(), "Id", "Type", genreId);
            ViewData["clubId"] = new SelectList(_context.Set<Club>(), "Id", "Name", clubId);
            ViewData["areaId"] = new SelectList(_context.Set<Area>(), "Id", "Type", areaId);
            return View("HomePage", await PartyWebAppContext.ToListAsync());
        }


        [HttpPost]
        [Authorize]
        public IActionResult SearchByPriceDate(double? price, DateTime dateInput)
        {
            return RedirectToAction(nameof(SearchByPriceDateResult), new { price = price, dateInput = dateInput });
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SearchByPriceDateResult(double? price, DateTime dateInput)
        {
            initTypeUserToViewData(returnCurrentUser());
            var PartyWebAppContext = _context.Party.Include(p => p.genre)
               .Include(p => p.area).Include(p => p.partyImage)
                .Where(p => p.price <= price && p.eventDate.Year.Equals(dateInput.Year)
                && p.eventDate.Month.Equals(dateInput.Month));


            ViewData["genreId"] = new SelectList(_context.Set<Genre>(), "Id", "Type");
            ViewData["clubId"] = new SelectList(_context.Set<Club>(), "Id", "Name");
            ViewData["areaId"] = new SelectList(_context.Set<Area>(), "Id", "Type");
            return View("HomePage", await PartyWebAppContext.ToListAsync());
        }


        [Authorize]
        public IActionResult MyParties()
        {
            ViewData["PageName"] = "My Parties";
            initTypeUserToViewData(returnCurrentUser());
            const UserType clientPermissionLevel = UserType.Client;
            bool isProducer = returnCurrentUser().Type > clientPermissionLevel;
            if (isProducer)
            {
                var partyWebAppContext = _context.Party
                  .Include(p => p.area)
           .Include(p => p.club)
           .Include(p => p.genre)
           .Include(p => p.partyImage)
           .Include(p => p.performers).Where(party => party.ProducerId == returnCurrentUser().Id);

                return View(partyWebAppContext.ToList());
            }
            else
            {
                var partyWebAppContext = _context.Party
                .Include(p => p.area)
         .Include(p => p.club)
         .Include(p => p.genre)
         .Include(p => p.partyImage)
         .Include(p => p.performers).Where(party => party.users.Contains(returnCurrentUser()));

                return View(partyWebAppContext.ToList());
            }
        }

        [Authorize]
        public IEnumerable<Party> mostPopularParties()
        {
            return (_partiesService.mostPopularParties());
        }

        // GET: Parties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            initTypeUserToViewData(returnCurrentUser());
            if (id == null)
            {
                return NotFound();
            }

            var party = await _context.Party
                .Include(p => p.area)
                .Include(p => p.club)
                .Include(p => p.genre)
                .Include(p => p.partyImage)
                .Include(p => p.performers)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (party == null)
            {
                return NotFound();
            }
            if (party.performers != null && party.performers.Any())
            {
                List<string> performersSpotifyIds = party.performers.Select(performer => performer.SpotifyId).ToList();
                var artists = await _spotifyClientService.GetArtists(performersSpotifyIds);
                ViewBag.artists = artists.ToArray();
            }
            ViewData["Genres"] = new SelectList(_context.Set<Genre>(), "Id", "Type");
            ViewData["Clubs"] = new SelectList(_context.Set<Club>(), "Id", "Name");
            ViewData["Areas"] = new SelectList(_context.Set<Area>(), "Id", "Type");

            return View(party);
        }

        // GET: Parties/Create

        [Authorize(Roles = "Admin, producer")]
        public IActionResult Create()
        {
            ViewData["Genres"] = new SelectList(_context.Set<Genre>(), "Id", "Type");
            ViewData["Clubs"] = new SelectList(_context.Set<Club>(), "Id", "Name");
            ViewData["Areas"] = new SelectList(_context.Set<Area>(), "Id", "Type");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, producer")]
        public async Task<IActionResult> Create([Bind("Id,name,eventDate,minimalAge,startTime,genreId,areaId,clubId,ProducerId,maxCapacity,price,ticketsPurchased")] Party party, string imageUrl, List<string> performersId)
        {
            bool isPerformersSubmitted = performersId != null && performersId.Any();
            if (ModelState.IsValid)
            {
                party.ProducerId = findCurrentUserId();

                party.ticketsPurchased = 0;
                if (isPerformersSubmitted)
                {
                    party.performers = new List<Performer>();
                    _partiesService.addPerformersToParty(party, performersId);
                }
                _partiesService.addImageToParty(party, imageUrl);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Genres"] = new SelectList(_context.Set<Genre>(), "Id", "Type");
            ViewData["Clubs"] = new SelectList(_context.Set<Club>(), "Id", "Name");
            ViewData["Areas"] = new SelectList(_context.Set<Area>(), "Id", "Type");

            return View(party);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Payment(int? id)
        {
            initTypeUserToViewData(returnCurrentUser());
            if (id == null)
            {
                return NotFound();
            }
            var party = await _context.Party.FindAsync(id);
            if (party == null)
            {
                return NotFound();
            }

            return View(party);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Payment(int id, int numOfTickets)
        {
            var party = _context.Party.FirstOrDefault(p => p.Id == id);
            User currentUser = returnCurrentUser();
            if (party == null)
            {
                return NotFound();
            }
            if (numOfTickets > 0)
            {
                Party updatedPartyWithTickets = _partiesService.addTicketsCountToParty(party, numOfTickets, currentUser);
                _context.Update(updatedPartyWithTickets);
                _context.Update(currentUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyParties));
            }
            else
            {
                ViewData["Error"] = "One ticket At least";
            }
            return View(party);
        }

        [Authorize(Roles = "Admin, producer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var party = await _context.Party.FindAsync(id);
            if (party == null)
            {
                return NotFound();
            }
            ViewData["Areas"] = new SelectList(_context.Set<Area>(), nameof(Area.Id), nameof(Area.Type));
            ViewData["Clubs"] = new SelectList(_context.Set<Club>(), nameof(Club.Id), nameof(Club.Name));
            ViewData["Genres"] = new SelectList(_context.Set<Genre>(), nameof(Genre.Id), nameof(Genre.Type));
            if (party.ProducerId != returnCurrentUser().Id)
            {
                return RedirectToAction("AccessDenied", "Users");
            }

            return View(party);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, producer")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,eventDate,minimalAge,startTime,genreId,areaId,clubId,ProducerId,maxCapacity,price,ticketsPurchased")] Party party)
        {
            ViewData["Areas"] = new SelectList(_context.Set<Area>(), nameof(Area.Id), nameof(Area.Type));
            ViewData["Clubs"] = new SelectList(_context.Set<Club>(), nameof(Club.Id), nameof(Club.Name));
            ViewData["Genres"] = new SelectList(_context.Set<Genre>(), nameof(Genre.Id), nameof(Genre.Type));
            if (id != party.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(party);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PartyExists(party.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MyParties));
            }
            return View(party);
        }

        // GET: Parties/Delete/5
        [Authorize(Roles = "Admin, producer")]
        public async Task<IActionResult> Delete(int? id)
        {
            initTypeUserToViewData(returnCurrentUser());
            if (id == null)
            {
                return NotFound();
            }

            var party = await _context.Party
                .Include(p => p.area)
                .Include(p => p.club)
                .Include(p => p.genre)
                .Include(p => p.partyImage)
                .Include(p => p.performers)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (party == null)
            {
                return NotFound();
            }

            return View(party);
        }

        //Get
        [Authorize]
        public async Task<IActionResult> buyTickets(int? id, int count)
        {

            if (id == null)
            {
                return NotFound();
            }
            var party = await _context.Party
                .Include(p => p.area)
                .Include(p => p.club)
                .Include(p => p.genre)
                .Include(p => p.partyImage)
                .Include(p => p.performers)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (party == null)
            {
                return NotFound();
            }
            return View(party);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> buyTickets(int id, int count)
        {
            var currentUserId = HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = _context.User.FirstOrDefault(u => u.Id == Int32.Parse(currentUserId));

            var party = await _context.Party.FindAsync(id);
            party.users.Add(currentUser);
            party.ticketsPurchased += count;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(party);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PartyExists(party.Id))
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
            return View(party);
        }

        // POST: Parties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, producer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var party = await _context.Party.FindAsync(id);
            _context.Party.Remove(party);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<string> GetArtistIdBySearchParams(string queryParams)
        {
            return await _spotifyClientService.getArtistIdBySearchParams(queryParams);
        }

        private bool PartyExists(int id)
        {
            return _context.Party.Any(e => e.Id == id);
        }
        
    }
}
