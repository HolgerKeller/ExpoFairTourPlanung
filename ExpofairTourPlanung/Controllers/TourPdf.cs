using Microsoft.AspNetCore.Mvc;
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
using iText.Kernel.Colors;
using iText.IO.Font.Constants;
using iText.Layout.Renderer;
using iText.Layout.Layout;
using iText.Kernel.Pdf.Canvas;
using ExpofairTourPlanung.Data;
using Microsoft.Extensions.Logging;
using ExpofairTourPlanung.Models;
using iText.Layout.Borders;
using System.Text.RegularExpressions;
using System.Text;

namespace ExpofairTourPlanung.Controllers
{
    public class TourPdf : Controller
    {

        private readonly ILogger<TourPdf> _logger;

        static private EasyjobDbContext _context;

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


                if (!string.IsNullOrEmpty(tourFromDb.Comment) || !string.IsNullOrEmpty(tourFromDb.Team) ) {

                    Table mainTable1 = new Table(UnitValue.CreatePercentArray(new float[] { 20, 60, 20 })).UseAllAvailableWidth();

                    if (!string.IsNullOrEmpty(tourFromDb.Comment)) 
                    {
                        Cell cell1 = getCell(1, 3, "").Add(formatContent(tourFromDb.Comment)).SetBorderBottom(Border.NO_BORDER);
                        mainTable1.AddCell(cell1);
                    }

                    if (!string.IsNullOrEmpty(tourFromDb.Team))
                    {
                        Cell cell2 = getCell(1, 3, "").Add(formatContent("Team: " + getStaffNames(tourFromDb.Team))).SetBorderBottom(Border.NO_BORDER);
                        mainTable1.AddCell(cell2);
                    }

                    doc.Add(mainTable1);
                }

                Table mainTable = new Table(UnitValue.CreatePercentArray(new float[] { 15, 65, 20 })).UseAllAvailableWidth().SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1));

                mainTable.AddHeaderCell(new Paragraph(new Text("Uhrzeit").SetBold()));

                Cell cellHeader = new Cell(1, 2);
                cellHeader.Add(new Paragraph(new Text("Arbeitsplan").SetBold()));
                mainTable.AddHeaderCell(cellHeader);


                Cell cellTime = new Cell().SetBorderBottom(Border.NO_BORDER); ;
                Cell cellContent = new Cell().SetBorder(Border.NO_BORDER).SetPaddingBottom(5);
                Cell cellAdresse = new Cell().SetBorder(Border.NO_BORDER).SetBorderRight(new SolidBorder(ColorConstants.BLACK, 1));

                cellTime = getCell(1, 1, "TIME");
                cellContent = getCell(1, 1, "CONTENT");
                cellAdresse = getCell(1, 1, "ADR");


                _logger.LogInformation("TourNR" + tourFromDb.IdTour );



                cellTime.Add(formatContent("##" + tourFromDb.StartWorkDay + "##"));
                cellAdresse.Add(new Paragraph(""));
                cellContent.Add(formatContent("##ARBEITSBEGINN##"));

                mainTable.AddCell(cellTime);
                mainTable.AddCell(cellContent);
                mainTable.AddCell(cellAdresse);


                int cellColor = 0;

                Color greyColor = new DeviceRgb(242, 243, 244);

                foreach (var job in jobs)
                {

                    _logger.LogInformation("JobNR: " + job.IdJob);



                    cellTime = getCell(1,1,"TIME");
                    cellContent = getCell(1,1,"CONTENT");
                    cellAdresse = getCell(1,1,"ADR");



                    //                   if (cellColor == 1)

                    if (job.InOut.Equals("OUT"))
                    {
                        cellTime.SetBackgroundColor(greyColor);
                        cellContent.SetBackgroundColor(greyColor);
                        cellAdresse.SetBackgroundColor(greyColor);
                        cellColor = 0;
                    }
                    else
                    {
                        cellColor++;
                    }

                   if (!String.IsNullOrEmpty(job.HeadLine))
                    {
                        Cell cellTime1 = getCell(1,1,"TIME");
                        Cell cellContent1 = getCell(1,1,"CONTENT");
                        Cell cellAdresse1 = getCell(1,1,"ADR");

                        cellTime1.Add(new Paragraph(""));
                        cellAdresse1.Add(new Paragraph(""));
                        cellContent1.Add(formatContent(job.HeadLine));

                        mainTable.AddCell(cellTime1);
                        mainTable.AddCell(cellContent1);
                        mainTable.AddCell(cellAdresse1);
                    }

                    string time = "";

                    if (!String.IsNullOrEmpty(job.Time))
                    {
                        time = time + job.Time + "\n";
                    }
                    time = time + job.DeliveryType + "\n";
                    time = time + job.Number + "\n";
                    if (job.JobDateReturn != null)
                    {
                        time = time + "(" + job.JobDateReturn?.ToString("dd.MM.yyyy") + ")\n";
                    }

                    cellTime.Add(formatContent(time));

                    string content = "";

                    if (!String.IsNullOrEmpty(job.Caption)) content += "##" + job.Caption + "##" + "\n";

                    cellContent.Add(formatContent(content));


                    // if (!String.IsNullOrEmpty(job.Stock)) content += job.Stock;

                    string[] lines = job.Stock.Split(
                    new[] { "\r\n", "\r", "\n" },StringSplitOptions.None);

                    string factor = "";
                    string caption = "";
                    string factorOld = "";
                    string captionOld = "";

                    Table stockTable = new Table(UnitValue.CreatePercentArray(new float[] { 10, 90 })).UseAllAvailableWidth();

                    stockTable.SetBorder(Border.NO_BORDER);


                    _logger.LogInformation("lines.length: " + lines.Length);




                    if (lines.Length > 0)
                    {


                        int flagAbZeile2 = 0;
                        foreach (string line in lines)
                        {

                            _logger.LogInformation("line: " + line);

                            _logger.LogInformation("line HEX: " + BitConverter.ToString(Encoding.Default.GetBytes(line)));

                            string[] words = line.Split(
                                new[] { "\t" }, StringSplitOptions.None);

                            _logger.LogInformation("words.length: " + words.Length);


                            _logger.LogInformation("words[0]: " + words[0]);



                            if (words.Length > 1)
                            {
                                factor = words[0];
                                caption = words[1];
                                _logger.LogInformation("words[1]: " + words[1]);
                            

                                if(flagAbZeile2 == 0)
                                {
                                    factorOld = factor;
                                }

                                if(factor != "" && flagAbZeile2 == 1)
                                {
                                    Cell cell1 = new Cell(1, 1).Add(new Paragraph(factorOld)).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT);
                                    Cell cell2 = new Cell(1, 1).Add(new Paragraph(captionOld)).SetBorder(Border.NO_BORDER);

                                    stockTable.AddCell(cell1);
                                    stockTable.AddCell(cell2);
                                    captionOld = "";
                                    factorOld = factor;

                                }
                                captionOld += caption;
                                flagAbZeile2 = 1;

                            }
                        }
                    }

                    Cell cell11 = new Cell(1, 1).Add(new Paragraph(factorOld)).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT);
                   Cell cell21 = new Cell(1, 1).Add(new Paragraph(captionOld)).SetBorder(Border.NO_BORDER).SetPadding(2);

                    stockTable.AddCell(cell11);
                    stockTable.AddCell(cell21);

                    cellContent.Add(stockTable);


                    content = "";

                    if (!String.IsNullOrEmpty(job.Comment)) content += job.Comment;

                    cellContent.Add(formatContent(content));

                    //cellContent.Add(formatContent(content));

                    string address = "";

                    //if (!String.IsNullOrEmpty(job.Address))
                    //{
                    //    string[] words = job.Address.Split(';');

                    //    if (!String.IsNullOrEmpty(words[0])) address += words[0];
                    //    if (!String.IsNullOrEmpty(words[1])) address += "\n" + words[1];
                    //    if (!String.IsNullOrEmpty(words[4])) address += "\n" + words[4];
                    //    if (!String.IsNullOrEmpty(words[3])) address += "\n" + words[2] + " " + words[3];
                    //}

                    if (!String.IsNullOrEmpty(job.AddressTXT))
                    {
                        address = Regex.Replace(job.AddressTXT, "(\r \r|\r+)", "\r");
                    }

                    if (!String.IsNullOrEmpty(job.ReadyTime))
                    {
                        address = address + "\n fertig bis: " + job.ReadyTime + " Uhr";

                    }

                    cellAdresse.Add(formatContent(address));

                    mainTable.AddCell(cellTime);
                    mainTable.AddCell(cellContent);
                    mainTable.AddCell(cellAdresse);
                }

                doc.Add(mainTable);

                if (!string.IsNullOrEmpty(tourFromDb.Footer))
                {

                    Table mainTable1 = new Table(UnitValue.CreatePercentArray(new float[] { 20, 60, 20 })).UseAllAvailableWidth();

                    Cell cell1 = getCell(1, 3,"").Add(formatContent(tourFromDb.Footer));

                    mainTable1.AddCell(cell1);

                    doc.Add(mainTable1);

                }

                doc.Flush();
                doc.Close();
                pdfBytes = stream.ToArray();
            }

            var fileResult = new FileContentResult(pdfBytes, "application/pdf");
            fileResult.FileDownloadName = "Tour" + "_" + tourFromDb.TourDate.ToString("ddMMyyyy") + "_" + tourFromDb.TourNr.ToString() + ".pdf";

            return (fileResult);
        }

        static string getStaffNames(string StaffNumbers)
        {
            string StaffNames = "";
            if (StaffNumbers != null)
            {
                string[] numbers = StaffNumbers.Split(',');
                if (numbers != null)
                {
                    foreach (var num in numbers)
                    {
                        var employees = _context.Staffs.Where(x => x.EmployeeNr == num).ToList();
                        foreach (var employee in employees)
                        {
                            StaffNames = StaffNames + employee.EmployeeName1 + ',';
                        }
                    }
                    StaffNames = StaffNames.Remove(StaffNames.Length - 1, 1);
                }
            }
            return StaffNames;
        }

        Paragraph formatContent(string content)
        {

            Paragraph para = new();

            string bold = "##"; // Bold
            string red = "**";  // Red

            bool containsBold = content.Contains(bold);
            bool containsRed = content.Contains(red);

            if (!containsBold && !containsRed)
            {
                para.Add(content);
                return para;
            }

            string[] boldSeparatingStrings = { "##" };
            string[] words = content.Split(boldSeparatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

            bool isBold = content.StartsWith("##");

            foreach (var word in words)
            {
                if (isBold)
                {
                    // word is bold
                    para.Add(new Text(word).SetBold());
                    isBold = false;
                } else
                {
                    para.Add(word);
                    isBold = true;
                }
            }
            return para;
        }

        Cell getCell( int rowspan, int colspan, string type ) {

            int defaultFontSize = 10;


            if (type == "TIME")
            {
                return new Cell().SetFontSize(defaultFontSize).SetBorder(Border.NO_BORDER).SetBorderLeft(new SolidBorder(ColorConstants.BLACK, 1)).SetBorderRight(new SolidBorder(ColorConstants.BLACK, 1));
            }
            else if (type == "CONTENT")
            {
                return new Cell().SetFontSize(defaultFontSize).SetBorder(Border.NO_BORDER).SetPaddingBottom(5);
            } 
            else if (type == "ADR")
            {
                return new Cell().SetFontSize(defaultFontSize).SetBorder(Border.NO_BORDER).SetBorderRight(new SolidBorder(ColorConstants.BLACK, 1));
            } else
            {
                return new Cell(rowspan, colspan).SetFontSize(defaultFontSize);
            }
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

                var culture = new System.Globalization.CultureInfo("de-DE");

                table = new Table(UnitValue.CreatePercentArray(new float[] { 3, 3, 3, 2 })).UseAllAvailableWidth().SetMarginBottom(5);

                Cell cell1 = new Cell().Add(new Paragraph("Fahrer").SetFontSize(6)).SetVerticalAlignment(VerticalAlignment.TOP).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                Div div1 = new Div().SetTextAlignment(TextAlignment.CENTER).SetPadding(0);
                div1.Add(new Paragraph(TourPdf.getStaffNames(tour.Driver)).SetFontSize(12));
                cell1.Add(div1);
                table.AddCell(cell1);

                Cell cell2 = new Cell().Add(new Paragraph("Datum").SetFontSize(6).SetVerticalAlignment(VerticalAlignment.TOP)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                cell2.Add(new Paragraph(tour.TourDate.ToString("dd.MM.yyyy")).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.BOTTOM).SetHeight(20));
                table.AddCell(cell2);

                Cell cell3 = new Cell().Add(new Paragraph("Wochentag").SetFontSize(6).SetVerticalAlignment(VerticalAlignment.TOP)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                cell3.Add(new Paragraph(culture.DateTimeFormat.GetDayName(tour.TourDate.DayOfWeek)).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.BOTTOM).SetHeight(20));
                table.AddCell(cell3);

                Cell cell4 = new Cell(2,1).Add(new Paragraph("LKW").SetFontSize(6).SetVerticalAlignment(VerticalAlignment.TOP)).SetHorizontalAlignment(HorizontalAlignment.CENTER);

                if(!String.IsNullOrEmpty(tour.VehicleNr))
                {
                    cell4.Add(new Paragraph(tour.VehicleNr).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.BOTTOM).SetHeight(20));
                }
                table.AddCell(cell4);

                Cell cell12 = new Cell().Add(new Paragraph("Verantwortlich").SetFontSize(6)).SetVerticalAlignment(VerticalAlignment.TOP).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetHeight(36);
                Div div2 = new Div().SetTextAlignment(TextAlignment.CENTER).SetPadding(0).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetFontSize(10);
                div2.Add(new Paragraph(TourPdf.getStaffNames(tour.Master)).SetFontSize(12));
                cell12.Add(div2);
                table.AddCell(cell12);

                Cell cell22 = new Cell(1,2).Add(new Paragraph("Tourname").SetFontSize(6)).SetVerticalAlignment(VerticalAlignment.TOP).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetHeight(36);
                Div div22 = new Div().SetTextAlignment(TextAlignment.CENTER).SetPadding(0).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetFontSize(10);
                div22.Add(new Paragraph(tour.TourName).SetFontSize(12));
                cell22.Add(div22);
                table.AddCell(cell22);

            }
        }

    }

}
