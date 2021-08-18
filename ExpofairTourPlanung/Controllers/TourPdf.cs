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

namespace ExpofairTourPlanung.Controllers
{
    public class TourPdf : Controller
    {
        [HttpGet]
        public ActionResult CreateTourPdf(int id)
        {
            byte[] pdfBytes;
            using (var stream = new System.IO.MemoryStream())
            using (var wri = new PdfWriter(stream))
            using (var pdf = new PdfDocument(wri))
            using (var doc = new Document(pdf))
            {


                TableHeaderEventHandler handler = new TableHeaderEventHandler(doc);
                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, handler);

                // Calculate top margin to be sure that the table will fit the margin.
                float topMargin = 20 + handler.GetTableHeight();
                doc.SetMargins(topMargin, 36, 36, 36);

                for (int i = 0; i < 50; i++)
                {
                    doc.Add(new Paragraph("Hello World!"));
                }

                doc.Add(new AreaBreak());
                doc.Add(new Paragraph("Hello World!"));
                doc.Add(new AreaBreak());
                doc.Add(new Paragraph("Hello World!"));

                //                doc.Add(new Paragraph("Das wird das PDF für die Expofair Tourplanung für ID:" + id.ToString()));

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

            public TableHeaderEventHandler(Document doc)
            {
                this.doc = doc;
                InitTable();

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

            private void InitTable()
            {
                table = new Table(1).UseAllAvailableWidth();
                table.AddCell("Header row 1");
                table.AddCell("Header row 2");
                table.AddCell("Header row 3");
            }
        }

    }

}
