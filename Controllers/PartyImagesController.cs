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
    public class PartyImagesController : Controller
    {
        private readonly PartyWebAppContext _context;
        private readonly PartiesService _partiesService;

        public PartyImagesController(PartyWebAppContext context, PartiesService service)
        {
            _context = context;
            _partiesService = service;
        }
        
        // GET: PartyImages
        public async Task<IActionResult> Index()
        {
            var partyWebAppContext = _context.PartyImage.Include(p => p.Party);
            return View(await partyWebAppContext.ToListAsync());
        }

        // GET: PartyImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var partyImage = await _context.PartyImage
                .Include(p => p.Party)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (partyImage == null)
            {
                return NotFound();
            }

            return View(partyImage);
        }

        // GET: PartyImages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, producer")]
        public IActionResult Create()
        {
            ViewData["PartyId"] = new SelectList(_context.Party, "Id", "Id");
            return View(nameof(Edit));
        }

        // POST: PartyImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, producer")]
        public async Task<IActionResult> Create([Bind("Id,imageUrl")] PartyImage partyImage, Party image)
        {
          
            if (ModelState.IsValid)
            {
                _context.Add(partyImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PartyId"] = new SelectList(_context.Party, nameof(partyImage.PartyId), nameof(partyImage.PartyId), partyImage.Party.name);
            return View(partyImage);
        }

        // GET: PartyImages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, producer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var partyImage = await _context.PartyImage
                           .Include(p => p.Party)
                           .FirstOrDefaultAsync(m => m.Id == id);
            if (partyImage == null)
            {
                return NotFound();
            }
            ViewData["PartyId"] = new SelectList(_context.Party, nameof(partyImage.PartyId), nameof(partyImage.PartyId), partyImage.Party.name);
            return View(partyImage);
        }

        // POST: PartyImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, producer")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,imageUrl,PartyId")] PartyImage partyImage)
        {
            if (id != partyImage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(partyImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PartyImageExists(partyImage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("myParties", "Parties");
            }
            ViewData["PartyId"] = new SelectList(_context.Party, nameof(partyImage.PartyId), nameof(partyImage.PartyId), partyImage.Party.name);
            return View(partyImage);
        }
        
        // GET: PartyImages/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, producer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var partyImage = await _context.PartyImage
                .Include(p => p.Party)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (partyImage == null)
            {
                return NotFound();
            }

            return View(partyImage);
        }

        // POST: PartyImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var partyImage = await _context.PartyImage.FindAsync(id);
            _context.PartyImage.Remove(partyImage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PartyImageExists(int id)
        {
            return _context.PartyImage.Any(e => e.Id == id);
        }
    }
}
