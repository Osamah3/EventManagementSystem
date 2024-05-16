using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Trion.Data;
using Trion.Models;

namespace Trion.Controllers
{
    public class RegistrationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager; 

        public RegistrationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }
       

        // GET: Registrations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Registration.Include(r => r.User);
            return View(await applicationDbContext.ToListAsync());
        }


        [Authorize]


        [HttpGet]
        public IActionResult BookTicket(int eventId)
        {
            var @event = _context.Event.FirstOrDefault(e => e.Id == eventId);

            if (@event == null)
            {

                return NotFound();
            }

            var model = new Registration
            {
                EventId = eventId,
            };

            return View(model);
        }








        [Authorize]
        [HttpPost]
        public async Task<IActionResult> BookTicket(Registration model, DateOnly now)
        {
            
                var user = await _userManager.GetUserAsync(User);
                

             if (user != null)
             {
                var existingRegistration = _context.Registration.FirstOrDefault(r => r.EventId == model.EventId && r.UserID == user.Id);

                if (existingRegistration != null)
                {
                    ViewBag.ErrorMessage = "You have already booked a ticket for this event.";
                    return View(model);
                }
                else
                {

                    model.UserID = user.Id;
                    model.Date = now;
                    model.PhoneNumber = model.PhoneNumber;
                    model.RegistrationName = model.RegistrationName;
                    model.Status = RegistrationStatus.Registered;

                    // Add registration to the database
                    _context.Registration.Add(model);
                    await _context.SaveChangesAsync();

                    ViewBag.SuccessMessage = "Your booking has been confirmed.";

                    return View(model);
                }
             }
                   return View(model);
        }

        [HttpGet]
        public IActionResult Booking(int eventId)
        {

            var bookings = _context.Registration
           .Where(r => r.EventId == eventId)
           .Include(r => r.User)
           .ToList();
            var totalCount = bookings.Count;
            ViewBag.TotalCount = totalCount;
            return View(bookings);
            
        }


        
        public IActionResult UpdateStatus(int registrationId, RegistrationStatus status)
        {
            if (!Enum.IsDefined(typeof(RegistrationStatus), status))
            {
                return BadRequest("Invalid status provided.");
            }
            var registration = _context.Registration.FirstOrDefault(r => r.Id == registrationId);
            if (registration == null)
            {
                return NotFound(); 
            }
            registration.Status = status;
            _context.Update(registration);
            _context.SaveChanges();
            return RedirectToAction("Index", "Events");

        }


        [HttpGet]
        [Authorize]
        public IActionResult MyEvents(int eventId)
        {

            var currentUser = _userManager.GetUserAsync(User).Result;

            var eventIds = _context.Registration
                .Where(r => r.UserID == currentUser.Id && r.Status == RegistrationStatus.Registered)
                .Select(r => r.EventId)
                .ToList();

            var events = _context.Event
             .Where(e => eventIds.Contains(e.Id))
                 .ToList();


            return View(events);

        }

        public IActionResult ConfirmCancel(int eventId)
        {
            // Assume user is logged in and you fetch user details as below:
            var currentUser = _userManager.GetUserAsync(User).Result;

            // Check if the registration exists
            var registration = _context.Registration.FirstOrDefault(r => r.EventId == eventId && r.UserID == currentUser.Id);
            if (registration == null)
            {
                return NotFound();
            }

            // Pass the eventID to the confirmation view, possibly within a ViewModel if more data is required
            return View("ConfirmCancel", eventId);
        }


        [HttpPost]
        public IActionResult CancelRegistration(int eventId)
        {
            var currentUser = _userManager.GetUserAsync(User).Result;

            var registration = _context.Registration.FirstOrDefault(r => r.EventId == eventId && r.UserID == currentUser.Id);
            if (registration == null)
            {
                return NotFound();
            }

            registration.Status = RegistrationStatus.Canceled; // Update the status to Canceled
            _context.Update(registration);
            _context.SaveChanges();
            TempData["Message"] = "Your registration has been canceled.";
            return RedirectToAction("MyEvents"); // Or any other page you'd redirect after cancellation
        }
    }



    //// GET: Registrations/Details/5
    //public async Task<IActionResult> Details(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return NotFound();
    //        }

    //        var registration = await _context.Registration
    //            .Include(r => r.User)
    //            .FirstOrDefaultAsync(m => m.Id == id);
    //        if (registration == null)
    //        {
    //            return NotFound();
    //        }

    //        return View(registration);
    //    }

    //    // GET: Registrations/Create
    //    public IActionResult Create()
    //    {
    //        ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id");
    //        return View();
    //    }

    //    // POST: Registrations/Create
    //    // To protect from overposting attacks, enable the specific properties you want to bind to.
    //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<IActionResult> Create([Bind("Id,EventId,UserID,Date,Status")] Registration registration)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            _context.Add(registration);
    //            await _context.SaveChangesAsync();
    //            return RedirectToAction(nameof(Index));
    //        }
    //        ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", registration.UserID);
    //        return View(registration);
    //    }

    //    // GET: Registrations/Edit/5
    //    public async Task<IActionResult> Edit(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return NotFound();
    //        }

    //        var registration = await _context.Registration.FindAsync(id);
    //        if (registration == null)
    //        {
    //            return NotFound();
    //        }
    //        ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", registration.UserID);
    //        return View(registration);
    //    }

    //    // POST: Registrations/Edit/5
    //    // To protect from overposting attacks, enable the specific properties you want to bind to.
    //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<IActionResult> Edit(int id, [Bind("Id,EventId,UserID,Date,Status")] Registration registration)
    //    {
    //        if (id != registration.Id)
    //        {
    //            return NotFound();
    //        }

    //        if (ModelState.IsValid)
    //        {
    //            try
    //            {
    //                _context.Update(registration);
    //                await _context.SaveChangesAsync();
    //            }
    //            catch (DbUpdateConcurrencyException)
    //            {
    //                if (!RegistrationExists(registration.Id))
    //                {
    //                    return NotFound();
    //                }
    //                else
    //                {
    //                    throw;
    //                }
    //            }
    //            return RedirectToAction(nameof(Index));
    //        }
    //        ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", registration.UserID);
    //        return View(registration);
    //    }

    //    // GET: Registrations/Delete/5
    //    public async Task<IActionResult> Delete(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return NotFound();
    //        }

    //        var registration = await _context.Registration
    //            .Include(r => r.User)
    //            .FirstOrDefaultAsync(m => m.Id == id);
    //        if (registration == null)
    //        {
    //            return NotFound();
    //        }

    //        return View(registration);
    //    }

    //    // POST: Registrations/Delete/5
    //    [HttpPost, ActionName("Delete")]
    //    [ValidateAntiForgeryToken]
    //    public async Task<IActionResult> DeleteConfirmed(int id)
    //    {
    //        var registration = await _context.Registration.FindAsync(id);
    //        if (registration != null)
    //        {
    //            _context.Registration.Remove(registration);
    //        }

    //        await _context.SaveChangesAsync();
    //        return RedirectToAction(nameof(Index));
    //    }

    //    private bool RegistrationExists(int id)
    //    {
    //        return _context.Registration.Any(e => e.Id == id);
    //    }
    //}
}
