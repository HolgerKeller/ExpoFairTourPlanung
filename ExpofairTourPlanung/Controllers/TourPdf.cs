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

        private readonly EasyjobDbContext _context;

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

                Cell cellHeader = new Cell(1, 2);
                cellHeader.Add(new Paragraph("Arbeitsplan"));
                mainTable.AddHeaderCell(cellHeader);

                Cell cellDatum = new Cell();
                Cell cellContent = new Cell().SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1));
                Cell cellAdresse = new Cell().SetBorderLeft(Border.NO_BORDER);

                cellDatum.Add(new Paragraph("24.08.2020"));
                cellContent.Add(new Paragraph("dsfsdfsfsfsfsfqweqeqeqeq"));
                cellAdresse.Add(new Paragraph("Start XXXX"));


                mainTable.AddCell(cellDatum);
                mainTable.AddCell(cellContent);
                mainTable.AddCell(cellAdresse);

                doc.Add(mainTable);

                doc.Flush();
                doc.Close();
                pdfBytes = stream.ToArray();
            }
            return new FileContentResult(pdfBytes, "application/pdf");
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
                cell11.SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1)).SetHeight(31);

                Div div1 = new Div().SetTextAlignment(TextAlignment.CENTER).SetPadding(0);
                div1.Add(new Paragraph("Max Muster").SetFontSize(10));
                cell11.Add(div1);
                subTable.AddCell(cell11);
  
                Cell cell12 = new Cell().Add(new Paragraph("Verantwortlich").SetFontSize(6)).SetVerticalAlignment(VerticalAlignment.TOP).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetBorder(Border.NO_BORDER).SetHeight(31);
               

                Div div2 = new Div().SetTextAlignment(TextAlignment.CENTER).SetPadding(0).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetFontSize(10);
                div2.Add(new Paragraph("Thomas Müller"));
                cell12.Add(div2);
                subTable.AddCell(cell12);


                table = new Table(4).UseAllAvailableWidth();

                Cell cell1 = new Cell().Add(subTable);
                cell1.SetPadding(0);
            
                table.AddCell(cell1);

                Cell cell2 = new Cell().Add(new Paragraph("Datum").SetFontSize(6).SetVerticalAlignment(VerticalAlignment.TOP)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                cell2.Add(new Paragraph(tour.TourDate.ToString("dd.MM.yyyy")).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.BOTTOM).SetHeight(20));
                table.AddCell(cell2);

                Cell cell3 = new Cell().Add(new Paragraph("Wochentag").SetFontSize(6).SetVerticalAlignment(VerticalAlignment.TOP)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                cell3.Add(new Paragraph("Montag").SetFontSize(10).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.BOTTOM).SetHeight(20));
                table.AddCell(cell3);

                Cell cell4 = new Cell().Add(new Paragraph("LKW").SetFontSize(6).SetVerticalAlignment(VerticalAlignment.TOP)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                cell4.Add(new Paragraph("B-HK1234").SetFontSize(10).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.BOTTOM).SetHeight(20));
                table.AddCell(cell4);


            }
        }

    }

}
