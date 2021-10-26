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
using ExpofairTourPlanung.Services;
using iText.IO.Image;

namespace ExpofairTourPlanung.Services
{
    public class DeliveryPdf : IDeliveryPdf
    {
        private readonly ILogger<DeliveryPdf> _logger;

        static private EasyjobDbContext _context;

        public DeliveryPdf(EasyjobDbContext context, ILogger<DeliveryPdf> logger)
        {
            _context = context;
            _logger = logger;
        }

        public string createDeliveryPdf( Del4Job del)
        {
            
            string pdfName = "c:\\temp\\Lieferung_" + del.IdTourJob + "_" + del.JobDate.ToString("yyyyMMddHHmmss") + ".pdf"; 

            using (var wri = new PdfWriter(pdfName))
            using (var pdf = new PdfDocument(wri))
            using (var doc = new Document(pdf, PageSize.A4, false))
            {

                TableHeaderEventHandler handler = new TableHeaderEventHandler(doc, del);
//                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, handler);

                // Calculate top margin to be sure that the table will fit the margin.
//                float topMargin = 20 + handler.GetTableHeight();
//                doc.SetMargins(topMargin, 20, 20, 20);

                Color greyColor = new DeviceRgb(242, 243, 244);


                doc.Add(new Paragraph("JobNummer:" + del.Number));
                doc.Add(new Paragraph("JobBez:" + del.Caption));
                doc.Add(new Paragraph("Status:" + del.Status));
                doc.Add(new Paragraph("Bemerkung:" + del.Comment));
                doc.Add(new Paragraph("Lieferzeitpunkt:" + del.DeliveryTime.ToString()));
                doc.Add(new Paragraph("Kundenname:" + del.Customer));

 

                if (!String.IsNullOrWhiteSpace(del.CustomerSignature))
                {
                    var base64Signature = del.CustomerSignature.Split(",")[1];
                    var binarySignature = Convert.FromBase64String(base64Signature);
                    ImageData imageData = ImageDataFactory.Create(binarySignature);

                    Image img = new Image(imageData);
                    doc.Add(img);

                   // System.IO.File.WriteAllBytes("Signature.png", binarySignature);
                }

                doc.Flush();
                doc.Close();
            }

            return (pdfName);
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


        private class TableHeaderEventHandler : IEventHandler
        {
            private Table table;
            private float tableHeight;
            private Document doc;
            private Del4Job del;

            public TableHeaderEventHandler(Document doc, Del4Job del)
            {
                this.doc = doc;
                this.del = del;

                InitHeaderTable(del);

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

            private void InitHeaderTable(Del4Job del)
            {
                var culture = new System.Globalization.CultureInfo("de-DE");

                table = new Table(UnitValue.CreatePercentArray(new float[] { 3, 3, 3, 2 })).UseAllAvailableWidth().SetMarginBottom(5);

                Cell cell1 = new Cell().Add(new Paragraph("Fahrer").SetFontSize(6)).SetVerticalAlignment(VerticalAlignment.TOP).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                Div div1 = new Div().SetTextAlignment(TextAlignment.CENTER).SetPadding(0);
                div1.Add(new Paragraph(del.HeadName)).SetFontSize(12);
                cell1.Add(div1);
                table.AddCell(cell1);

                Cell cell2 = new Cell().Add(new Paragraph("Datum").SetFontSize(6).SetVerticalAlignment(VerticalAlignment.TOP)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                cell2.Add(new Paragraph(del.JobDate.ToString("dd.MM.yyyy")).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.BOTTOM).SetHeight(20));
                table.AddCell(cell2);

                Cell cell3 = new Cell().Add(new Paragraph("Wochentag").SetFontSize(6).SetVerticalAlignment(VerticalAlignment.TOP)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                cell3.Add(new Paragraph("").SetFontSize(12).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.BOTTOM).SetHeight(20));
                table.AddCell(cell3);

                Cell cell4 = new Cell(2, 1).Add(new Paragraph("LKW").SetFontSize(6).SetVerticalAlignment(VerticalAlignment.TOP)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                if (!String.IsNullOrEmpty(""))
                {
                    cell4.Add(new Paragraph("").SetFontSize(12).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.BOTTOM).SetHeight(20));
                }
                table.AddCell(cell4);

                Cell cell12 = new Cell().Add(new Paragraph("Verantwortlich").SetFontSize(6)).SetVerticalAlignment(VerticalAlignment.TOP).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetHeight(36);
                Div div2 = new Div().SetTextAlignment(TextAlignment.CENTER).SetPadding(0).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetFontSize(10);
                div2.Add(new Paragraph(del.HeadName).SetFontSize(12));
                cell12.Add(div2);
                table.AddCell(cell12);

                Cell cell22 = new Cell(1, 2).Add(new Paragraph("Tourname").SetFontSize(6)).SetVerticalAlignment(VerticalAlignment.TOP).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetHeight(36);
                Div div22 = new Div().SetTextAlignment(TextAlignment.CENTER).SetPadding(0).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetFontSize(10);
                div22.Add(new Paragraph(del.Caption).SetFontSize(12));
                cell22.Add(div22);
                table.AddCell(cell22);
            }
        }
    }
}
