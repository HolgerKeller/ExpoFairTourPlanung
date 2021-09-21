﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iText.Layout;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.IO;
using iText.Kernel.Events;
using iText.Kernel.Geom;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Layout.Renderer;
using iText.Layout.Layout;
using iText.Kernel.Pdf.Canvas;
using ExpofairTourPlanung.Data;
using Microsoft.Extensions.Logging;
using ExpofairTourPlanung.Models;
using iText.Layout.Borders;
using iText.Kernel.Colors;

namespace ExpofairTourPlanung.Controllers
{
    public class TourPdf : Controller
    {

        private readonly ILogger<TourPdf> _logger;

        static private  EasyjobDbContext _context;

        public TourPdf(EasyjobDbContext context, ILogger<TourPdf> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult CreateTourPdf(int id)
        {
            byte[] pdfBytes;


            var tourFromDb = _context.Tours.SingleOrDefault(x => x.IdTour == id);
            var jobs = _context.Job2Tours.Where(x => x.IdTour == tourFromDb.IdTour).OrderBy(x => x.Ranking).ToList();

            using (var stream = new System.IO.MemoryStream())
            using (var wri = new PdfWriter(stream))
            using (var pdf = new PdfDocument(wri))
            using (var doc = new Document(pdf, PageSize.A4, false))
            {

                TableHeaderEventHandler handler = new TableHeaderEventHandler(doc, tourFromDb);
                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, handler);

                // Calculate top margin to be sure that the table will fit the margin.
                float topMargin = 20 + handler.GetTableHeight();
                doc.SetMargins(topMargin, 20, 20, 20);

                Table mainTable = new Table(UnitValue.CreatePercentArray(new float[] { 20, 60, 20 })).UseAllAvailableWidth();

                mainTable.AddHeaderCell(new Paragraph("Uhrzeit"));

                int defaultFontSize = 10;

                Cell cellHeader = new Cell(1, 2);
                cellHeader.Add(new Paragraph("Arbeitsplan"));
                mainTable.AddHeaderCell(cellHeader);

                //Cell cellContent = new Cell().SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1)).SetPaddingBottom(10);


                Cell cellTime = new Cell().SetBorderBottom(Border.NO_BORDER); ;
                Cell cellContent = new Cell().SetBorder(Border.NO_BORDER).SetPaddingBottom(5);
                Cell cellAdresse = new Cell().SetBorder(Border.NO_BORDER).SetBorderRight(new SolidBorder(ColorConstants.BLACK, 1));

                cellTime.Add(new Paragraph("05:30"));
                cellTime.SetFontSize(defaultFontSize);
                cellContent.Add(new Paragraph("Arbeitsbeginn:" + tourFromDb.TourName));
                cellContent.SetFontSize(defaultFontSize);
                cellAdresse.Add(new Paragraph(""));

                mainTable.AddCell(cellTime);
                mainTable.AddCell(cellContent);
                mainTable.AddCell(cellAdresse);

                cellTime = new Cell().SetFontSize(defaultFontSize).SetBorder(Border.NO_BORDER).SetBorderLeft(new SolidBorder(ColorConstants.BLACK, 1)).SetBorderRight(new SolidBorder(ColorConstants.BLACK, 1));
                cellContent = new Cell().SetFontSize(defaultFontSize).SetBorder(Border.NO_BORDER).SetPaddingBottom(5);
                cellAdresse = new Cell().SetFontSize(defaultFontSize).SetBorder(Border.NO_BORDER).SetBorderRight(new SolidBorder(ColorConstants.BLACK, 1));

                cellTime.Add(new Paragraph(""));
                cellContent.Add(new Paragraph("Team: " + getStuffNames(tourFromDb.Team)));
                cellAdresse.Add(new Paragraph(""));

                mainTable.AddCell(cellTime);
                mainTable.AddCell(cellContent);
                mainTable.AddCell(cellAdresse);

                if (tourFromDb.Comment != null)
                {
                    cellTime = new Cell().SetFontSize(defaultFontSize).SetBorder(Border.NO_BORDER).SetBorderLeft(new SolidBorder(ColorConstants.BLACK, 1)).SetBorderRight(new SolidBorder(ColorConstants.BLACK, 1));
                    cellContent = new Cell().SetFontSize(defaultFontSize).SetBorder(Border.NO_BORDER).SetPaddingBottom(5);
                    cellAdresse = new Cell().SetFontSize(defaultFontSize).SetBorder(Border.NO_BORDER).SetBorderRight(new SolidBorder(ColorConstants.BLACK, 1));

                    cellTime.Add(new Paragraph(""));
                    cellContent.Add(new Paragraph(tourFromDb.Comment));
                    cellAdresse.Add(new Paragraph(""));

                    mainTable.AddCell(cellTime);
                    mainTable.AddCell(cellContent);
                    mainTable.AddCell(cellAdresse);

                }

                int cellColor = 0;

                foreach (var job in jobs)
                {
                    
                    cellTime = new Cell().SetFontSize(defaultFontSize).SetPaddingBottom(5).SetBorder(Border.NO_BORDER).SetBorderLeft(new SolidBorder(ColorConstants.BLACK, 1)).SetBorderRight(new SolidBorder(ColorConstants.BLACK, 1));
                    cellContent = new Cell().SetFontSize(defaultFontSize).SetBorder(Border.NO_BORDER).SetPaddingBottom(5);
                    cellAdresse = new Cell().SetFontSize(defaultFontSize).SetBorder(Border.NO_BORDER).SetBorderRight(new SolidBorder(ColorConstants.BLACK, 1));

 
                    if (cellColor == 1)
                    {
                        cellTime.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                        cellContent.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                        cellAdresse.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                         cellColor = 0;
                    }
                    else
                    {
                        cellColor++;
                    }

                    string time = "";
                    if( job.Time != null )
                    {
                        time = time + job.Time + "\n";
                    }
                    time = time + job.InOut + "\n";
                    time = time + job.Number + "\n";
                    if(job.JobDateReturn != null)
                    {
                        time = time + "(" + job.JobDateReturn?.ToString("dd.MM.yyyy") + ")\n";
                    }

                    cellTime.Add(new Paragraph(time));

                    string content = "";

                    if (job.Stock != null) content = content + job.Stock;

                    if (job.Comment != null) content = content + job.Comment;


                    cellContent.Add(new Paragraph(content));

                    string address = "";

                    if (job.Address != null)
                    {
                        string[] words = job.Address.Split(';');
                        address = words[0] + "\n" + words[3] + "\n" + words[1] +" "+  words[2] ;
                    }

                    if( job.ReadyTime != null )
                    {
                        address = address + "\n fertig bis:" + job.ReadyTime;

                    }

                    cellAdresse.Add(new Paragraph(address));

                    mainTable.AddCell(cellTime);
                    mainTable.AddCell(cellContent);
                    mainTable.AddCell(cellAdresse);

 
                }


                Cell cellTableEnd = new Cell(1, 3).SetBorderTop(Border.NO_BORDER).Add(new Paragraph(""));
                mainTable.AddCell(cellTableEnd);

                doc.Add(mainTable);

                doc.Flush();
                doc.Close();
                pdfBytes = stream.ToArray();
            }
            return new FileContentResult(pdfBytes, "application/pdf");
        }

        static string getStuffNames(string StuffNumbers)
        {
            string StuffNames = "";
            if (StuffNumbers != null)
            {
                string[] numbers = StuffNumbers.Split(',');
                if (numbers != null)
                {
                    foreach (var num in numbers)
                    {
                        var employees = _context.Stuffs.Where(x => x.EmployeeNr == num).ToList();
                        foreach (var employee in employees)
                        {
                            StuffNames = StuffNames + employee.EmployeeName1 + ',';
                        }
                    }
                    StuffNames = StuffNames.Remove(StuffNames.Length - 1, 1);
                }
            }
            return StuffNames;
        }


        private class TableHeaderEventHandler : IEventHandler
        {
            private Table table;
            private float tableHeight;
            private Document doc;
            private Tour tour;

            public TableHeaderEventHandler(Document doc, Tour tour)
            {
                this.doc = doc;
                this.tour = tour;

                InitHeaderTable( tour );

                TableRenderer renderer = (TableRenderer)table.CreateRendererSubTree();
                renderer.SetParent(new DocumentRenderer(doc));

                // Simulate the positioning of the renderer to find out how much space the header table will occupy.
                LayoutResult result = renderer.Layout(new LayoutContext(new LayoutArea(0, PageSize.A4)));
                tableHeight = result.GetOccupiedArea().GetBBox().GetHeight();
            }

            public void HandleEvent(Event currentEvent)
            {
                PdfDocumentEvent docEvent = (PdfDocumentEvent)currentEvent;
                PdfDocument pdfDoc = docEvent.GetDocument();
                PdfPage page = docEvent.GetPage();
                PdfCanvas canvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
                PageSize pageSize = pdfDoc.GetDefaultPageSize();
                float coordX = pageSize.GetX() + doc.GetLeftMargin();
                float coordY = pageSize.GetTop() - doc.GetTopMargin();
                float width = pageSize.GetWidth() - doc.GetRightMargin() - doc.GetLeftMargin();
                float height = GetTableHeight();
                Rectangle rect = new Rectangle(coordX, coordY, width, height);

                new Canvas(canvas, rect)
                    .Add(table)
                    .Close();

            }


            public float GetTableHeight()
            {
                return tableHeight;
            }

            private void InitHeaderTable(Tour tour )
            {
 
                Table subTable = new Table(1).UseAllAvailableWidth();

                subTable.SetBorder(Border.NO_BORDER);

                Cell cell11 = new Cell().Add(new Paragraph("Fahrer").SetFontSize(6)).SetVerticalAlignment(VerticalAlignment.TOP).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetBorder(Border.NO_BORDER); ;
                cell11.SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1)).SetHeight(36);

                Div div1 = new Div().SetTextAlignment(TextAlignment.CENTER).SetPadding(0);
                div1.Add(new Paragraph(TourPdf.getStuffNames(tour.Driver)).SetFontSize(12));
                cell11.Add(div1);
                subTable.AddCell(cell11);
  
                Cell cell12 = new Cell().Add(new Paragraph("Verantwortlich").SetFontSize(6)).SetVerticalAlignment(VerticalAlignment.TOP).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetBorder(Border.NO_BORDER).SetHeight(36);
               

                Div div2 = new Div().SetTextAlignment(TextAlignment.CENTER).SetPadding(0).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetFontSize(10);
                div2.Add(new Paragraph(TourPdf.getStuffNames(tour.Master)).SetFontSize(12));
                cell12.Add(div2);
                subTable.AddCell(cell12);


                table = new Table(4).UseAllAvailableWidth();

                Cell cell1 = new Cell().Add(subTable);
                cell1.SetPadding(0);
            
                table.AddCell(cell1);

                Cell cell2 = new Cell().Add(new Paragraph("Datum").SetFontSize(6).SetVerticalAlignment(VerticalAlignment.TOP)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                cell2.Add(new Paragraph(tour.TourDate.ToString("dd.MM.yyyy")).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.BOTTOM).SetHeight(20));
                table.AddCell(cell2);

                var culture = new System.Globalization.CultureInfo("de-DE");
 
                Cell cell3 = new Cell().Add(new Paragraph("Wochentag").SetFontSize(6).SetVerticalAlignment(VerticalAlignment.TOP)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                cell3.Add(new Paragraph(culture.DateTimeFormat.GetDayName(tour.TourDate.DayOfWeek)).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.BOTTOM).SetHeight(20));
                table.AddCell(cell3);

                Cell cell4 = new Cell().Add(new Paragraph("LKW").SetFontSize(6).SetVerticalAlignment(VerticalAlignment.TOP)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                cell4.Add(new Paragraph(tour.VehicleNr).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.BOTTOM).SetHeight(20));
                table.AddCell(cell4);


            }
        }

    }

}
