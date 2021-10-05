using ExpofairTourPlanung.Data;
using ExpofairTourPlanung.Models;
using Microsoft.AspNetCore.Http;
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
    public class EventController : Controller
    {
        private readonly ILogger<EventController> _logger;

        private readonly EasyjobDbContext _context;

        public EventController(EasyjobDbContext context, ILogger<EventController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index(string dateFrom)
        {
            string dateTo = null;

            if (dateFrom == null)
            {

                dateFrom = this.HttpContext.Session.GetString("dateFrom");

                if (dateFrom == null)
                {
                    dateFrom = DateTime.Now.ToString("yyyy-MM-dd");
                }
            }

            dateTo = dateFrom;

            ViewData["dateFrom"] = dateFrom;
            this.HttpContext.Session.SetString("dateFrom", dateFrom);

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




            var dateFromParam1 = new SqlParameter()
            {
                ParameterName = "@Date",
                SqlDbType = System.Data.SqlDbType.VarChar,
                Direction = System.Data.ParameterDirection.Input,
                Size = 10,
                Value = dateFrom
            };


            var allEvents = _context.ExpoEvents.FromSqlRaw("exec expofair.GetExpoEvents @Date", dateFromParam1).ToList();



            return View(allEvents);
        }

        public IActionResult CreateTour(string date, string eventList)
        {
            ViewData["dateFrom"] = date;

            var dateFromParam = new SqlParameter()
            {
                ParameterName = "@EventDate",
                SqlDbType = System.Data.SqlDbType.VarChar,
                Direction = System.Data.ParameterDirection.Input,
                Size = 10,
                Value = date
            };

            var stringEvent = new SqlParameter()
            {
                ParameterName = "@EventString",
                SqlDbType = System.Data.SqlDbType.VarChar,
                Direction = System.Data.ParameterDirection.Input,
                Size = 4000,
                Value = eventList
            };

            var createTour = _context.Database.ExecuteSqlRaw("exec expofair.CreateTourFromEvents @EventDate, @EventString", dateFromParam, stringEvent);


            return RedirectToAction("Index", new { dateFrom = date });
        }
    }
}

