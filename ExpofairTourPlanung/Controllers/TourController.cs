using ExpofairTourPlanung.Data;
using ExpofairTourPlanung.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ExpofairTourPlanung.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using iText.Layout;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.IO;

namespace ExpofairTourPlanung.Controllers
{
    public class TourController : Controller
    {

        private readonly EasyjobDbContext _context;
        private readonly ILogger<TourController> _logger;
       
        public TourController(EasyjobDbContext context, ILogger<TourController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private SelectList _getAllVehicles ( DateTime date )
        {

            //DateTime date = DateTime.Now;

            var vehicles = _context.Vehicles.Where(x => x.IsActiv == true && ( x.StartDate <= date && x.EndDate >= date )).ToList();

            var vehicleSelectItems = new List<VehicleSelectItem>();
            foreach (var vehicle in vehicles)
            {
                vehicleSelectItems.Add(new VehicleSelectItem { VehicleNr = vehicle.VehicleNr, VehicleName = vehicle.VehicleNr });
            }
            return new SelectList(vehicleSelectItems, "VehicleNr", "VehicleName");
        }

        private SelectList _getStuff( DateTime date )
        {

           // DateTime date = DateTime.Now;

            var employees = _context.Stuffs.Where(x => x.IsActiv == true && (x.StartDate <= date && x.EndDate >= date)).ToList();

            var employeeSelectItems = new List<EmployeeSelectItem>();
            foreach (var employee in employees)
            {
                employeeSelectItems.Add(new EmployeeSelectItem { EmployeeNr = employee.EmployeeNr, EmployeeName = employee.EmployeeName1 });
            }
            return new SelectList(employeeSelectItems, "EmployeeNr", "EmployeeName");
        }

        private SelectList _getTourNames()
        {

            int i = 1;

            var tourNameSelectItems = new List<TourNameSelectItem>();

            while (i < 9) 
            {
                String tour = "Tour " + i.ToString();
                tourNameSelectItems.Add(new TourNameSelectItem { TourName = tour, TourDesc = tour });
                i++;
            }
            return new SelectList(tourNameSelectItems, "TourName", "TourDesc");
        }



        public IActionResult CreateEditTour( int id)
        {

            ViewBag.Tournames = _getTourNames();

            if ( id != 0)
            {

                var tourFromDb = _context.Tours.SingleOrDefault(x => x.IdTour == id);

                //if ((jobPostingFromDb.OwnerUsername != User.Identity.Name) && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized();
                //}

                ViewData["FreeJobsCount"] = 0;
                ViewData["TourJobsCount"] = 0;


                if (tourFromDb != null)
                {

                    DateTime tourdate = tourFromDb.TourDate;

                    var dateFromParam = new SqlParameter()
                    {
                        ParameterName = "@DateStart",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        Direction = System.Data.ParameterDirection.Input,
                        Size = 10,
                        Value = tourdate.ToString()
                    };

                    var dateToParam = new SqlParameter()
                    {
                        ParameterName = "@DateEnd",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        Direction = System.Data.ParameterDirection.Input,
                        Size = 10,
                        Value = tourdate.ToString()
                    };

                    var CopyJobs = _context.Database.ExecuteSqlRaw("exec expofair.CustCopyJobsByDate @DateStart, @DateEnd", dateFromParam, dateToParam);

                    ViewBag.Vehicles = _getAllVehicles( tourdate );
                    ViewBag.Employees = _getStuff( tourdate );

                    var freeJobsFromDb = _context.Job2Tours.Where(x => x.IdTour != id && x.JobDate == tourdate && x.Service != "Selbstabholer" ).OrderByDescending(x => x.InOut).ThenBy(x => x.Time).ToList();
                    var tourJobsFromDb = _context.Job2Tours.Where(x => x.IdTour == id && x.JobDate == tourdate ).OrderBy(x => x.Ranking).ToList();

                    ViewBag.TourJobs= tourJobsFromDb;
                    ViewBag.FreeJobs = freeJobsFromDb;

                    ViewData["FreeJobsCount"] = freeJobsFromDb.Count;
                    ViewData["TourJobsCount"] = tourJobsFromDb.Count;

                    return View(tourFromDb);
                }
                else
                {
                    return NotFound();
                }
            }
            return View();
        }

 

        public IActionResult SaveTour(Tour tour)
        {

            if (!ModelState.IsValid)
            {

                return RedirectToAction("CreateEditTour", new { id = tour.IdTour });

            }


            if (tour.IdTour == 0)
            {
                // Add new Tour if not editing

                //tour.TourDate = DateTime.Now;
                //tour.TourName = "Tour " + DateTime.Now.ToString("dd.MM.yyyy");
                //tour.CreateTime = DateTime.Now;

                _context.Tours.Add(tour);
            }
            else
            {
                var tourFromDb = _context.Tours.SingleOrDefault(x => x.IdTour == tour.IdTour);

                if (tourFromDb == null)
                {
                    return NotFound();
                }

                if (tour.TourName == null)
                {
                    tour.TourName = tour.TourDate + " " + tour.VehicleNr;
                }

                tourFromDb.TourName = tour.TourName;
                tourFromDb.TourDate = tour.TourDate;
                tourFromDb.VehicleNr = tour.VehicleNr;
                tourFromDb.Driver = tour.Driver;
                tourFromDb.Comment = tour.Comment;
                tourFromDb.Team = tour.Team;
                tourFromDb.Master = tour.Master;

            }

            _context.SaveChanges();

            return RedirectToAction("CreateEditTour", new { id = tour.IdTour });
        }

        [HttpPost]
        public IActionResult AddJob2Tour(int id, int idTourJob )
        {
            if (id == 0 || idTourJob == 0)
                return BadRequest();

            var idTourParam = new SqlParameter()
            {
                ParameterName = "@IdTour",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Input,
                Size = 10,
                Value = id
            };

            var idJobParam = new SqlParameter()
            {
                ParameterName = "@IdTourJob",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Input,
                Size = 10,
                Value = idTourJob
            };

            var CopyJobs = _context.Database.ExecuteSqlRaw("exec expofair.CustAddJobToTour @IdTour, @IdTourJob ", idTourParam, idJobParam);

            return Ok();
        }
        [HttpPost]
        public IActionResult DelJobFromTour(int id, int idJob)
        {
            if (id == 0 || idJob == 0)
                return BadRequest();

            var idTourParam = new SqlParameter()
            {
                ParameterName = "@IdTour",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Input,
                Size = 10,
                Value = id
            };

            var idJobParam = new SqlParameter()
            {
                ParameterName = "@IdJob",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Input,
                Size = 10,
                Value = idJob
            };

            var CopyJobs = _context.Database.ExecuteSqlRaw("exec expofair.CustDelJobFromTour @IdTour, @IdJob ", idTourParam, idJobParam);

            return Ok();
        }
        [HttpPost]
        public IActionResult IncreaseJobRanking(int id, int idJob)
        {
            if (id == 0 || idJob == 0)
                return BadRequest();

            var idTourParam = new SqlParameter()
            {
                ParameterName = "@IdTour",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Input,
                Size = 10,
                Value = id
            };

            var idJobParam = new SqlParameter()
            {
                ParameterName = "@IdJob",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Input,
                Size = 10,
                Value = idJob
            };

            var CopyJobs = _context.Database.ExecuteSqlRaw("exec expofair.CustIncreaseJobRanking @IdTour, @IdJob ", idTourParam, idJobParam);

            return Ok();
        }

        [HttpPost]
        public IActionResult DecreaseJobRanking(int id, int idJob)
        {
            if (id == 0 || idJob == 0)
                return BadRequest();

            var idTourParam = new SqlParameter()
            {
                ParameterName = "@IdTour",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Input,
                Size = 10,
                Value = id
            };

            var idJobParam = new SqlParameter()
            {
                ParameterName = "@IdJob",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Input,
                Size = 10,
                Value = idJob
            };

            var CopyJobs = _context.Database.ExecuteSqlRaw("exec expofair.CustDecreaseJobRanking @IdTour, @IdJob ", idTourParam, idJobParam);

            return Ok();
        }

        [HttpGet]
        public IActionResult GetJobDetail(int id)
        {
            if (id == 0)
                return BadRequest();

            var jobFromDb = _context.Job2Tours.SingleOrDefault(x => x.IdTourJob == id);

            if (jobFromDb == null)
                return NotFound();

            return Ok(jobFromDb);
        }



    }
}
