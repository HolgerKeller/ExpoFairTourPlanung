using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpofairTourPlanung.Data;
using ExpofairTourPlanung.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace ExpofairTourPlanung.Controllers
{
    public class SbController : Controller
    {
        private readonly EasyjobDbContext _context;
        private readonly ILogger<TourListController> _logger;

        public SbController(EasyjobDbContext context, ILogger<TourListController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {

            var allTours = _context.Tours.Where(x => x.IsSbtour == true).OrderByDescending(x => x.TourDate).ToList();

            return View(allTours);
        }

        [HttpPost]
        public IActionResult DelTourById(int id)
        {
            if (id == 0)
                return BadRequest();

            var idTourParam = new SqlParameter()
            {
                ParameterName = "@IdTour",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Input,
                Size = 10,
                Value = id
            };

            var CopyJobs = _context.Database.ExecuteSqlRaw("exec expofair.CustDeleteTour @IdTour", idTourParam);

            return Ok();
        }

        public IActionResult CreateSbTour(int id)
        {
            return View();
        }

        public IActionResult SaveSbTour(Tour tour)
        {


            DateTime tourdate = tour.TourDate;

            var dateFromParam = new SqlParameter()
            {
                ParameterName = "@SbDate",
                SqlDbType = System.Data.SqlDbType.VarChar,
                Direction = System.Data.ParameterDirection.Input,
                Size = 10,
                Value = tourdate.ToString()
            };

            var CopyJobs = _context.Database.ExecuteSqlRaw("exec expofair.CreateSBTour @SbDate", dateFromParam);

            return RedirectToAction("Index");
        }

        public IActionResult ShowDetailSB( int id )
        {


            var tourFromDb = _context.Tours.SingleOrDefault(x => x.IdTour == id);

            if (tourFromDb != null)
            {

                var sBJobsFromDb = _context.Job2Tours.Where(x => x.IdTour == id ).OrderBy(x => x.Ranking).ToList();

                ViewBag.sBJobs = sBJobsFromDb;


                return View(tourFromDb);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
