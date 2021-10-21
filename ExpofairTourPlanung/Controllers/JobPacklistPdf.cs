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
using Microsoft.EntityFrameworkCore;
using iText.Forms;
using iText.Forms.Fields;

namespace ExpofairTourPlanung.Controllers
{
    public class JobPacklistPdf : Controller
    {

        private readonly ILogger<TourPdf> _logger;

        static private EasyjobDbContext _context;

        public JobPacklistPdf(EasyjobDbContext context, ILogger<TourPdf> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult CreateJobStockPdf(int id)
        {
            byte[] pdfBytes;

//            TODO Job Holen

            var jobFromDb = _context.Job2Tours.SingleOrDefault(x => x.IdJob == id && x.InOut == "OUT");

            var jobList = new Microsoft.Data.SqlClient.SqlParameter()
            {
                ParameterName = "@Jobs",
                SqlDbType = System.Data.SqlDbType.VarChar,
                Direction = System.Data.ParameterDirection.Input,
                Size = 10,
                Value = id.ToString()
            };

            var allStock = _context.Stock2JobSPs.FromSqlRaw("exec expofair.CreatePacklistByJobs @Jobs", jobList);

            using (var stream = new System.IO.MemoryStream())
            using (var wri = new PdfWriter(stream))
            using (var pdf = new PdfDocument(wri))
            using (var doc = new Document(pdf, PageSize.A4, false))
            {

                TableHeaderEventHandler handler = new TableHeaderEventHandler(doc, jobFromDb);
                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, handler);

                // Calculate top margin to be sure that the table will fit the margin.
                float topMargin = 20 + handler.GetTableHeight();
                doc.SetMargins(topMargin, 20, 20, 20);

                Table mainTable = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10, 60, 10, 10 })).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                mainTable.AddHeaderCell(new Paragraph(new Text("Anzahl").SetBold()));
                mainTable.AddHeaderCell(new Paragraph(new Text("Artikel").SetBold()));
                mainTable.AddHeaderCell(new Paragraph(new Text("Material").SetBold()));
                mainTable.AddHeaderCell(new Paragraph(new Text("Anfang").SetBold()));
                mainTable.AddHeaderCell(new Paragraph(new Text("Bereit").SetBold()));

                Cell cellCount = new Cell();
                Cell cellNumber = new Cell();
                Cell cellContent = new Cell();
                Cell cellStart = new Cell();
                Cell cellReady = new Cell();

                int cellColor = 0;
                Color greyColor = new DeviceRgb(242, 243, 244);

                int lfd = 0;
                foreach (var stock in allStock)
                {

                    lfd++;

                    cellCount = getCell(1, 1, "RIGHTBORDER");
                    cellNumber = getCell(1, 1, "RIGHTBORDER");
                    cellContent = getCell(1, 1, "NOBORDER");
                    cellStart = getCell(1, 1, "LEFTBORDER");
                    cellReady = getCell(1, 1, "LEFTBORDER");

                    Div divCount = new Div().SetTextAlignment(TextAlignment.RIGHT).SetPadding(0).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetMarginRight(5).SetFontSize(10);
                    divCount.Add(new Paragraph(stock.Count.ToString()));
                    cellCount.Add(divCount);

                    Div divNumber = new Div().SetTextAlignment(TextAlignment.RIGHT).SetPadding(0).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetMarginRight(5).SetFontSize(10);
                    divNumber.Add(new Paragraph(stock.CustomNumber));
                    cellNumber.Add(divNumber);

                    Div divContent = new Div().SetTextAlignment(TextAlignment.LEFT).SetPadding(0).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetMarginLeft(5).SetFontSize(10);
                    divContent.Add(new Paragraph(stock.Caption));
                    cellContent.Add(divContent);

                    cellStart.Add(new Paragraph(""));
                    cellReady.Add(new Paragraph(""));

                    cellStart.SetNextRenderer(new CheckboxCellRenderer(cellStart, "St" + lfd));
                    cellReady.SetNextRenderer(new CheckboxCellRenderer(cellReady, "Rd" + lfd));

                    if (cellColor == 1)
                    {
                        cellCount.SetBackgroundColor(greyColor);
                        cellNumber.SetBackgroundColor(greyColor);
                        cellContent.SetBackgroundColor(greyColor);
                        cellReady.SetBackgroundColor(greyColor);
                        cellStart.SetBackgroundColor(greyColor);
                        cellColor = 0;
                    }
                    else
                    {
                        cellColor++;
                    }

                    mainTable.AddCell(cellCount);
                    mainTable.AddCell(cellNumber);
                    mainTable.AddCell(cellContent);
                    mainTable.AddCell(cellStart);
                    mainTable.AddCell(cellReady);

                }

                doc.Add(mainTable);

                Cell cellTableEnd = new Cell(1, 4).SetBorderTop(Border.NO_BORDER).Add(formatContent(""));
                mainTable.AddCell(cellTableEnd);

                doc.Flush();
                doc.Close();
                pdfBytes = stream.ToArray();
            }
            var fileResult = new FileContentResult(pdfBytes, "application/pdf");
            fileResult.FileDownloadName = "Packliste" + "_" + jobFromDb.JobDate.ToString("ddMMyyyy") + "_" + jobFromDb.IdJob.ToString() + ".pdf";

            return (fileResult);
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
                }
                else
                {
                    para.Add(word);
                    isBold = true;
                }
            }

            return para;
        }

        Cell getCell(int rowspan, int colspan, string type)
        {

            int defaultFontSize = 10;


            if (type == "RIGHTBORDER")
            {
                return new Cell().SetFontSize(defaultFontSize).SetHorizontalAlignment(HorizontalAlignment.RIGHT).SetBorder(Border.NO_BORDER).SetBorderRight(new SolidBorder(ColorConstants.BLACK, 1));
            }
            else if (type == "NOBORDER")
            {
                return new Cell().SetFontSize(defaultFontSize).SetBorder(Border.NO_BORDER).SetPaddingBottom(5);
            }
            else if (type == "LEFTBORDER")
            {
                return new Cell().SetFontSize(defaultFontSize).SetBorder(Border.NO_BORDER).SetBorderLeft(new SolidBorder(ColorConstants.BLACK, 1));
            }
            else
            {
                return new Cell(rowspan, colspan).SetFontSize(defaultFontSize);
            }
        }

        private class CheckboxCellRenderer : CellRenderer
        {
            // The name of the check box field
            protected internal String name;

            public CheckboxCellRenderer(Cell modelElement, String name)
                : base(modelElement)
            {
                this.name = name;
            }

            // If renderer overflows on the next area, iText uses getNextRender() method to create a renderer for the overflow part.
            // If getNextRenderer isn't overriden, the default method will be used and thus a default rather than custom
            // renderer will be created
            public override IRenderer GetNextRenderer()
            {
                return new CheckboxCellRenderer((Cell)GetModelElement(), name);
            }

            public override void Draw(DrawContext drawContext)
            {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(drawContext.GetDocument(), true);

                // Define the coordinates of the middle
                float x = (GetOccupiedAreaBBox().GetLeft() + GetOccupiedAreaBBox().GetRight()) / 2;
                float y = (GetOccupiedAreaBBox().GetTop() + GetOccupiedAreaBBox().GetBottom()) / 2;

                // Define the position of a check box that measures 20 by 20
                Rectangle rect = new Rectangle(x - 10, y - 10, 20, 20);

                // The 4th parameter is the initial value of checkbox: 'Yes' - checked, 'Off' - unchecked
                // By default, checkbox value type is cross.
                PdfButtonFormField checkBox = PdfFormField.CreateCheckBox(drawContext.GetDocument(), rect, name, "Off");
                form.AddFieldAppearanceToPage(checkBox, drawContext.GetDocument().GetPage(GetOccupiedArea().GetPageNumber()));
            }
        }

        private class TableHeaderEventHandler : IEventHandler
        {
            private Table table;
            private float tableHeight;
            private Document doc;
            private Job2Tour job;

            public TableHeaderEventHandler(Document doc, Job2Tour job)
            {
                this.doc = doc;
                this.job = job;

                InitHeaderTable(job);

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

            private void InitHeaderTable(Job2Tour job)
            {
                var culture = new System.Globalization.CultureInfo("de-DE");

                table = new Table(UnitValue.CreatePercentArray(new float[] { 2, 3, 3, 3 })).UseAllAvailableWidth().SetMarginBottom(5);

                Cell cell1 = new Cell().Add(new Paragraph("JobNr").SetFontSize(6)).SetVerticalAlignment(VerticalAlignment.TOP).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                Div div1 = new Div().SetTextAlignment(TextAlignment.CENTER).SetPadding(0);
                div1.Add(new Paragraph(job.Number).SetFontSize(10));
                cell1.Add(div1);
                table.AddCell(cell1);

                Cell cell2 = new Cell().Add(new Paragraph("Job-Datum").SetFontSize(6).SetVerticalAlignment(VerticalAlignment.TOP)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                cell2.Add(new Paragraph(job.JobDate.ToString("dd.MM.yyyy")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.BOTTOM).SetHeight(20));
                table.AddCell(cell2);

                Cell cell3 = new Cell().Add(new Paragraph("JobTyp").SetFontSize(6).SetVerticalAlignment(VerticalAlignment.TOP)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                cell3.Add(new Paragraph(job.DeliveryType).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.BOTTOM).SetHeight(20));
                table.AddCell(cell3);

                Cell cell4 = new Cell(2, 1).Add(new Paragraph("Adresse").SetFontSize(6).SetVerticalAlignment(VerticalAlignment.TOP)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                if (!String.IsNullOrEmpty(job.AddressTXT))
                {
                    cell4.Add(new Paragraph(job.AddressTXT).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.BOTTOM).SetHeight(20));
                }
                table.AddCell(cell4);

                Cell cell12 = new Cell().Add(new Paragraph("Projektleiter").SetFontSize(6)).SetVerticalAlignment(VerticalAlignment.TOP).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetHeight(36);
                Div div2 = new Div().SetTextAlignment(TextAlignment.CENTER).SetPadding(0).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetFontSize(10);
                div2.Add(new Paragraph(job.UserName).SetFontSize(10));
                cell12.Add(div2);
                table.AddCell(cell12);

                Cell cell22 = new Cell(1, 2).Add(new Paragraph("Job").SetFontSize(6)).SetVerticalAlignment(VerticalAlignment.TOP).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetHeight(36);
                Div div22 = new Div().SetTextAlignment(TextAlignment.CENTER).SetPadding(0).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetFontSize(10);
                div22.Add(new Paragraph(job.Caption).SetFontSize(10));
                cell22.Add(div22);
                table.AddCell(cell22);
            }
        }
    }
}