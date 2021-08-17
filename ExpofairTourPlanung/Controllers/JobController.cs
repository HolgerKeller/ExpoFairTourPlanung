using ExpofairTourPlanung.Data;
using ExpofairTourPlanung.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpofairTourPlanung.Controllers
{
    public class JobController : Controller
    {
        private readonly EasyjobDbContext _context;
        private readonly ILogger<TourController> _logger;

        public JobController(EasyjobDbContext context, ILogger<TourController> logger)
        {
            _context = context;
            _logger = logger;
        }


        public IActionResult EditJob(int id)
        {

            if (id != 0)
            {
                var jobFromDb = _context.Job2Tours.SingleOrDefault(x => x.IdTourJob == id);

                //if ((jobPostingFromDb.OwnerUsername != User.Identity.Name) && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized();
                //}

           
                if (jobFromDb != null)
                {

                    return View(jobFromDb);
                }
                else
                {
                    return NotFound();
                }
            }
            return View();
        }
        public IActionResult SaveJob(Job2Tour job)
        {

            int IdTour = 0;

            if (job.IdTourJob == 0)
            {
                _context.Job2Tours.Add(job);
            }
            else
            {
                var jobFromDb = _context.Job2Tours.SingleOrDefault(x => x.IdTourJob == job.IdTourJob);

                if (jobFromDb == null)
                {
                    return NotFound();
                }


                jobFromDb.Time = job.Time;
                jobFromDb.Comment = job.Comment;
                jobFromDb.Stock = job.Stock;

                IdTour = jobFromDb.IdTour.Value;


            }

            _context.SaveChanges();

            return RedirectToAction("CreateEditTour", "Tour", new { id = IdTour });
        }
    }
}
