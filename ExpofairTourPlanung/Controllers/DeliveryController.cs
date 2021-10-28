using ExpofairTourPlanung.Data;
using ExpofairTourPlanung.Models;
using ExpofairTourPlanung.Models.ViewModels;
using ExpofairTourPlanung.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace ExpofairTourPlanung.Controllers
{
    public class DeliveryController : Controller
    {
        private readonly EasyjobDbContext _context;
        private readonly ILogger<DeliveryController> _logger;
        private readonly IDeliveryPdf _deliveryPdf;

        public DeliveryController(EasyjobDbContext context, ILogger<DeliveryController> logger, IDeliveryPdf deliveryPdf)
        {
            _context = context;
            _logger = logger;
            _deliveryPdf = deliveryPdf;
        }

        private SelectList _getLsStatus()
        {




            var deliveryStatusSelectItems = new List<DeliveryStatusSelectItem>();

            deliveryStatusSelectItems.Add(new DeliveryStatusSelectItem { Status = "LS_OK", Description = StatCodes.StatusCodeDic["LS_OK"] });
            deliveryStatusSelectItems.Add(new DeliveryStatusSelectItem { Status = "LS_ANM", Description = StatCodes.StatusCodeDic["LS_ANM"] });
            deliveryStatusSelectItems.Add(new DeliveryStatusSelectItem { Status = "LS_NOK", Description = StatCodes.StatusCodeDic["LS_NOK"] });

         
            return new SelectList(deliveryStatusSelectItems, "Status", "Description");
        }

        private SelectList _getRlsStatus()
        {

            var deliveryStatusSelectItems = new List<DeliveryStatusSelectItem>();

            deliveryStatusSelectItems.Add(new DeliveryStatusSelectItem { Status = "RLS_OK", Description = StatCodes.StatusCodeDic["RLS_OK"] });
            deliveryStatusSelectItems.Add(new DeliveryStatusSelectItem { Status = "RLS_NOK", Description = StatCodes.StatusCodeDic["RLS_NOK"] });

            return new SelectList(deliveryStatusSelectItems, "Status", "Description");
        }





        public IActionResult List(string dateFrom)
        {

            if (dateFrom == null)
            {
                dateFrom = this.HttpContext.Session.GetString("dateFrom");

                if (dateFrom == null)
                {
                    dateFrom = DateTime.Now.ToString("yyyy-MM-dd");
                }
                //                return View();
            }


            ViewData["dateFrom"] = dateFrom;

            this.HttpContext.Session.SetString("dateFrom", dateFrom);


            _logger.LogInformation("dateFrom:" + dateFrom);

            DateTime dateFromDT = DateTime.Parse(dateFrom);

            var allTours = _context.Tours.Where(x => x.IsSbtour != true && x.TourDate == dateFromDT).OrderByDescending(x => x.TourDate).ToList();

            return View(allTours);
        }

        public IActionResult OpenTour(int id)
        {

            if (id != 0)
            {

                var tourFromDb = _context.Tours.SingleOrDefault(x => x.IdTour == id);

                //if ((jobPostingFromDb.OwnerUsername != User.Identity.Name) && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized();
                //}

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

                    var tourJobsFromDb = _context.Job2Tours.Where(x => x.IdTour == id && x.JobDate == tourdate).OrderBy(x => x.Ranking).ToList();

                    ViewBag.TourJobs = tourJobsFromDb;

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

        public IActionResult CheckJob(int id)
        {

            if (id != 0)
            {
                var delFromDb = _context.Del4Jobs.SingleOrDefault(x => x.IdTourJob == id);

                //if ((jobPostingFromDb.OwnerUsername != User.Identity.Name) && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized();
                //}


                if (delFromDb != null)
                {
                    if (delFromDb.InOut == "OUT")
                    {
                        ViewBag.Status = _getLsStatus();
                    }
                    else
                    {
                        ViewBag.Status = _getRlsStatus();
                    }


                    return View(delFromDb);
                }
                else
                {
                    var jobFromDb = _context.Job2Tours.SingleOrDefault(x => x.IdTourJob == id);

                    if (jobFromDb != null)
                    {
                        Del4Job delivery = new Del4Job();
                        delivery.IdTourJob = jobFromDb.IdTourJob;
                        delivery.Number = jobFromDb.Number;
                        delivery.IdJob = jobFromDb.IdJob;
                        delivery.Caption = jobFromDb.Caption;
                        delivery.JobDate = jobFromDb.JobDate;
                        delivery.ReadyTime = jobFromDb.ReadyTime;
                        delivery.Contact = jobFromDb.Contact;
                        delivery.ContactPhone = jobFromDb.ContactPhone;
                        delivery.UserEmail = jobFromDb.UserEmail;
                        delivery.UserName = jobFromDb.UserName;
                        delivery.Time = jobFromDb.Time;
                        delivery.LastUpdate = jobFromDb.LastUpdate;
                        delivery.InOut = jobFromDb.InOut;
                        delivery.HeadName = "Max Muster";
                        delivery.HeadEmail = "Holger.Keller@nexos.de";

                        _context.Del4Jobs.Add(delivery);

                        _context.SaveChanges();


                        if (delivery.InOut == "OUT")
                        {
                            ViewBag.Status = _getLsStatus();
                        }
                        else
                        {
                            ViewBag.Status = _getRlsStatus();
                        }

                        return View(delivery);

                    }

                }
            }
            return NotFound();
        }


        public IActionResult processJob(Del4Job delivery)
        {


            var delFromDb = _context.Del4Jobs.SingleOrDefault(x => x.IdDelJob == delivery.IdDelJob);

            if (delFromDb == null)
            {
                return NotFound();
            }

            delFromDb.CustomerSignature = delivery.CustomerSignature;
            delFromDb.DeliveryTime = DateTime.Now;

            _context.SaveChanges();

            string pdfName = _deliveryPdf.createDeliveryPdf(delFromDb);

            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.To.Add(delFromDb.CustomerEmail);
            mail.From = new MailAddress("info@Nexos.de", "Nexos.de", System.Text.Encoding.UTF8);
            mail.Subject = "Lieferdokument Expofair Auftrag" + delFromDb.Number;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = "Expofair ist Happy";
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            System.Net.Mail.Attachment attachment;
            attachment = new System.Net.Mail.Attachment(pdfName);
            mail.Attachments.Add(attachment);

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("lab@nexos.de", "HXbMWm9vDL2n6S7n");
            client.Port = 587;
            client.Host = "smtp.ionos.de";
            client.EnableSsl = true;
            client.Send(mail);

            return View(delFromDb);
        }

        public IActionResult SaveShowJob(Del4Job delivery)
        {

            var delFromDb = _context.Del4Jobs.SingleOrDefault(x => x.IdDelJob == delivery.IdDelJob);

            if (delFromDb == null)
            {
                return NotFound();
            }

            delFromDb.Comment = delivery.Comment;
            delFromDb.Status = delivery.Status;
            delFromDb.Customer = delivery.Customer;
            delFromDb.CustomerEmail = delivery.CustomerEmail;
            delFromDb.PackMaterial = delivery.PackMaterial;

            _context.SaveChanges();


            ViewBag.PaMa = getPackMat(delivery.PackMaterial);


            return View(delFromDb);
        }


        [HttpGet]
        public IActionResult GetPackMat()
        {
            var packMats = _context.PackMats.ToList();

            if (packMats == null)
                return NotFound();

            return Ok(packMats);
        }

        public string getPackMat( string packMat)
        {
            string longPackMats = "";

            if (String.IsNullOrEmpty(packMat)) return null;

            string[] entries = packMat.Split(';');

                foreach (string entry in entries)
                {
                    string[] part = entry.Split(':');
                    if( part.Length == 2)
                    {
                    string caption = getArticleName(part[0]);
                    string fullEntry = part[1] + "\t" + part[0] + "\t" + caption;
                    longPackMats = longPackMats + fullEntry + "\n";
                    }
                }
  
            return longPackMats;
        }

        private string getArticleName( string article )
        {
            string name = "";

            var entries =_context.PackMats.SingleOrDefault(x => x.Article == article);

            name = entries.Caption;

            return (name);
        }

    }
}

