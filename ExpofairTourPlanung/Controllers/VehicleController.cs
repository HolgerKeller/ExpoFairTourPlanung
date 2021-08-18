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
    public class VehicleController : Controller
    {
        private readonly EasyjobDbContext _context;
        private readonly ILogger<TourController> _logger;

        public VehicleController(EasyjobDbContext context, ILogger<TourController> logger)
        {
            _context = context;
            _logger = logger;
        }


        public IActionResult CreateEditVehicle(int id)
        {

       if (id != 0)
            {
                var vehicleFromDb = _context.Vehicles.SingleOrDefault(x => x.IdVehicle == id);

                //if ((jobPostingFromDb.OwnerUsername != User.Identity.Name) && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized();
                //}

                if (vehicleFromDb != null)
                {

                    return View(vehicleFromDb);
                }
                else
                {
                    return NotFound();
                }
            }
            return View();

        }

        public IActionResult SaveVehicle(Vehicle vehicle)
        {

            if (!ModelState.IsValid)
            {

                return RedirectToAction("CreateEditVehicle", new { id = vehicle.IdVehicle });

            }


            if (vehicle.IdVehicle == 0)
            {
                // Add new Tour if not editing

                //tour.TourDate = DateTime.Now;
                //tour.TourName = "Tour " + DateTime.Now.ToString("dd.MM.yyyy");
                //tour.CreateTime = DateTime.Now;

                _context.Vehicles.Add(vehicle);
            }
            else
            {
                var vehicleFromDb = _context.Vehicles.SingleOrDefault(x => x.IdVehicle == vehicle.IdVehicle);

                if (vehicleFromDb == null)
                {
                    return NotFound();
                }


                vehicleFromDb.Comment = vehicle.Comment;
                vehicleFromDb.VehicleNr = vehicle.VehicleNr;
                vehicleFromDb.VehicleType = vehicle.VehicleType;
                vehicleFromDb.NetVolume = vehicle.NetVolume;
                vehicleFromDb.NetWeight = vehicle.NetWeight;
                vehicleFromDb.IsActiv = vehicle.IsActiv;
                vehicleFromDb.Status = vehicle.Status;
                vehicleFromDb.EndDate = vehicle.EndDate;
                vehicleFromDb.StartDate = vehicle.StartDate;
                vehicleFromDb.Owner = vehicle.Owner;
             
            }

            _context.SaveChanges();

            return RedirectToAction( "CreateEditVehicle", new { id = vehicle.IdVehicle });
        }

        [HttpPost]
        public IActionResult DelVehicleById(int id)
        {
            if (id == 0)
                return BadRequest();

            var vehicleFromDb = _context.Vehicles.SingleOrDefault(x => x.IdVehicle == id);

            if (vehicleFromDb == null)
                return NotFound();

            _context.Vehicles.Remove(vehicleFromDb);
            _context.SaveChanges();

            return Ok();
        }

        public IActionResult Index()
        {

            var allVehicle = _context.Vehicles.OrderByDescending(x => x.VehicleNr).ToList();

            return View(allVehicle);
        }
    }
}
