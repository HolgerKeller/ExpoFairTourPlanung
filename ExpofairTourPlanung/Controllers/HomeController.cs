using ExpofairTourPlanung.Data;
using ExpofairTourPlanung.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ExpofairTourPlanung.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly EasyjobDbContext _context;

        public HomeController(EasyjobDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

  

        public IActionResult Index(string dateFrom)
        {

            string dateTo = null;


            if (dateFrom == null)
            {
                dateFrom = DateTime.Now.ToString("yyyy-MM-dd");
            }

            if (dateTo == null)
            {
                dateTo = dateFrom;
            }

            ViewData["dateFrom"] = dateFrom;

            _logger.LogInformation("dateFrom:" + dateFrom + " dateTo:" + dateTo);



            var dateFromParam = new SqlParameter()
            {
                ParameterName = "@DateStart",
                SqlDbType = System.Data.SqlDbType.VarChar,
                Direction = System.Data.ParameterDirection.Input,
                Size = 10,
                Value = dateFrom
            };

            var dateToParam = new SqlParameter()
            {
                ParameterName = "@DateEnd",
                SqlDbType = System.Data.SqlDbType.VarChar,
                Direction = System.Data.ParameterDirection.Input,
                Size = 10,
                Value = dateTo
            };

            var CopyJobs = _context.Database.ExecuteSqlRaw("exec expofair.CustCopyJobsByDate @DateStart, @DateEnd", dateFromParam, dateToParam);

            DateTime dateFromDT = DateTime.Parse(dateFrom);
            DateTime dateToDT = DateTime.Parse(dateTo);

            var allJobs = _context.Job2Tours.Where(x => x.JobDate >= dateFromDT && x.JobDate <= dateToDT).ToList();

            _logger.LogInformation("Anzahl Jobs:" + allJobs.Count.ToString());


            return View(allJobs);
        }

        [HttpGet]
        public IActionResult GetJobDetail(int id)
        {
            if (id == 0)
                return BadRequest();

            var jobFromDb = _context.Job2Tours.SingleOrDefault(x => x.IdTourJob == id);

            if (jobFromDb == null)
                return NotFound();

            if (jobFromDb.Comment == null) jobFromDb.Comment = "";

            return Ok(jobFromDb);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
