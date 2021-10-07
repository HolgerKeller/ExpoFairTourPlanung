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
    public class StaffController : Controller
    {
        private readonly EasyjobDbContext _context;
        private readonly ILogger<TourController> _logger;

        public StaffController(EasyjobDbContext context, ILogger<TourController> logger)
        {
            _context = context;
            _logger = logger;
        }


        public IActionResult CreateEditEmployee(int id)
        {

            if (id != 0)
            {
                var employeeFromDb = _context.Staffs.SingleOrDefault(x => x.IdStaff == id);

                //if ((jobPostingFromDb.OwnerUsername != User.Identity.Name) && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized();
                //}

                if (employeeFromDb != null)
                {

                    return View(employeeFromDb);
                }
                else
                {
                    return NotFound();
                }
            }
            return View();

        }

        public IActionResult Reset(int id)
        {
            return RedirectToAction("CreateEditEmployee", 0);

        }

        public IActionResult SaveEmployee(Staff employee)
        {

            if (!ModelState.IsValid)
            {

                return RedirectToAction("CreateEditEmployee", new { id = employee.IdStaff });

            }


            if (employee.IdStaff == 0)
            {
          

                _context.Staffs.Add(employee);
            }
            else
            {
                var employeeFromDb = _context.Staffs.SingleOrDefault(x => x.IdStaff == employee.IdStaff);

                if (employeeFromDb == null)
                {
                    return NotFound();
                }


                employeeFromDb.Comments = employee.Comments;
                employeeFromDb.EmployeeNr = employee.EmployeeNr;
                employeeFromDb.EmployeeType = employee.EmployeeType;
                employeeFromDb.EmployeeName1 = employee.EmployeeName1;
                employeeFromDb.EmployeeName2 = employee.EmployeeName2;
                employeeFromDb.IsActiv = employee.IsActiv;
                employeeFromDb.Status = employee.Status;
                employeeFromDb.EndDate = employee.EndDate;
                employeeFromDb.StartDate = employee.StartDate;
                employeeFromDb.Employer = employee.Employer;

            }

            _context.SaveChanges();

            return RedirectToAction("CreateEditEmployee", new { id = employee.IdStaff });
        }

        [HttpPost]
        public IActionResult DelEmployeeById(int id)
        {
            if (id == 0)
                return BadRequest();

            var employeeFromDb = _context.Staffs.SingleOrDefault(x => x.IdStaff == id);

            if (employeeFromDb == null)
                return NotFound();

            _context.Staffs.Remove(employeeFromDb);
            _context.SaveChanges();

            return Ok();
        }

        public IActionResult Index()
        {

            var allemployee = _context.Staffs.OrderByDescending(x => x.EmployeeName1).ToList();

            return View(allemployee);
        }
    }
}
