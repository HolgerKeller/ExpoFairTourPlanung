using ExpofairTourPlanung.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpofairTourPlanung.Services
{
    public interface IDeliveryPdf
    {
        string createDeliveryPdf( Del4Job del );
    }
}
