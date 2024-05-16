using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Trion.Data;
using Trion.Models;

namespace Trion.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var events = await _context.Event.Include(e => e.Venue).ToListAsync();
            return View(events);
            //  return View(await _context.Event.ToListAsync());
        }

        // GET: Events/ShowSearchForm
        public async Task<IActionResult> ShowSearchForm()
        {
            return View();
        }
        //PoST: Jokes/ShowSearchResults
        public async Task<IActionResult> ShowSearchResults(string searchPhrase)
        {
            if (_context == null)
            {
                // Return an error response
                return StatusCode(500, "Internal Server Error. Database context is not initialized.");
            }

                var searchResults = await _context.Event
                .Include(e => e.Venue) // Include the Venue entity
                .Where(e => e.Name.Contains(searchPhrase))
                .ToListAsync();

                return View("Index", searchResults);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        [Authorize]
        public IActionResult Create()
        {
            var venues = _context.Venue.Select(v => new SelectListItem
            {
                Value = v.VenueId.ToString(),
                Text = v.VenueName
            }).ToList();

            ViewBag.Venues = venues;

            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Date,EventDescription,venueid")] Event @event)
        {
            
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            ViewBag.Venues = new SelectList(_context.Venue, "VenueId", "VenueName", @event.venueid);
            return View(@event);
        }

        //public async Task<IActionResult> Create([Bind("Id,Name,Date, EventDescription")] Event @event)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        _context.Add(@event);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(@event);
        //}

        // GET: Events/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            if (_context.Venue != null)
            {
                ViewBag.Venues = new SelectList(_context.Venue, "VenueId", "VenueName", @event.venueid);
            }
            else
            {
                ViewBag.Venues = new SelectList(new List<Venue>(), "VenueId", "VenueName");
            }
            return View(@event);

           // return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Date, EventDescription, venueid")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

           
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            
            ViewBag.Venues = new SelectList(_context.Venue, "VenueId", "VenueName", @event.venueid);
            return View(@event);
           // return View(@event);
        }

        // GET: Events/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Event.FindAsync(id);
            if (@event != null)
            {
                _context.Event.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.Id == id);
        }
    }
}
