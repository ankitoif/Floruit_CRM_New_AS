using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Model.ViewModels
{
    public class ServiceRequestDocument
    {
        public IFormFile File { get; set; }
        public long ServiceTypeDocumentId { get; set; }
    }
}
