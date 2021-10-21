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

namespace ExpofairTourPlanung.Helper
{
    public class DeliveryPdf
    {
        private readonly ILogger<DeliveryPdf> _logger;

        static private EasyjobDbContext _context;

        public DeliveryPdf(EasyjobDbContext context, ILogger<DeliveryPdf> logger)
        {
            _context = context;
            _logger = logger;
        }

        public int createDeliveryPdf( int id)
        {
            int status = 0;
            var delFromDb = _context.Del4Jobs.SingleOrDefault(x => x.IdDelJob == id);



            return (status);
        }





    }
}
