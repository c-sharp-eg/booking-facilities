﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using booking_facilities.Models;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;
using booking_facilities.Repositories;

namespace booking_facilities.Controllers
{
    [Authorize(AuthenticationSchemes = "oidc", Policy ="administrator")]
    public class VenuesController : Controller
    {
        private readonly IVenueRepository venueRepository;

        public VenuesController(IVenueRepository venueRepository)
        {
            this.venueRepository = venueRepository;
        }

        // GET: Venues
        public async Task<IActionResult> Index(int? page)
        {
            IQueryable<Venue> venues =  venueRepository.GetAllAsync().OrderBy(v => v.VenueName);

            venues = venues.OrderBy(v => v.VenueName);

            var venueList = await venues.ToListAsync();

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var venuesPerPage = 10;

            var onePageOfVenue = venueList.ToPagedList(pageNumber, venuesPerPage); // will only contain 25 products max because of the pageSize

            ViewBag.onePageOfVenue = onePageOfVenue;
            return View();
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await venueRepository.GetByIdAsync(id.Value);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueId,VenueName")] Venue venue)
        {
            if (venueRepository.DoesVenueExist(venue.VenueName))
            {
                ModelState.AddModelError("VenueName", "Venue already exists. Please enter another venue.");
            }
            else if (ModelState.IsValid)
            {
                await venueRepository.AddAsync(venue);
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await venueRepository.GetByIdAsync(id.Value);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        // POST: Venues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName")] Venue venue)
        {
            if (id != venue.VenueId)
            {
                return NotFound();
            }

            if (venueRepository.DoesVenueExist(venue.VenueName))
            {
                ModelState.AddModelError("VenueName", "Venue already exists. Please enter another venue.");
            }
            else if(ModelState.IsValid)
            {
                try
                {
                    await venueRepository.UpdateAsync(venue);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId))
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
            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await venueRepository.GetByIdAsync(id.Value);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await venueRepository.GetByIdAsync(id);
            await venueRepository.DeleteAsync(venue);
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return venueRepository.VenueExists(id);
        }
    }
}
