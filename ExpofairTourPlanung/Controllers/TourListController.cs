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
    public class TourListController : Controller
    {
        private readonly EasyjobDbContext _context;
        private readonly ILogger<TourListController> _logger;

        public TourListController(EasyjobDbContext context, ILogger<TourListController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {

            var allTours = _context.Tours.OrderByDescending(x => x.TourDate).ToList();

            return View( allTours );
        }

        [HttpPost]
        public IActionResult DelTourById(int id)
        {
            if (id == 0 )
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





    }
}
