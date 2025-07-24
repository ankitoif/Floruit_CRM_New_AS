using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Model.ViewModels
{
    public class ServiceTypeListViewModel
    {
        public long? ServiceTypeId { get; set; }
        public string ServiceTypeDescription { get; set; }
        public string Organisation { get; set; }
        public bool? IsValid { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
    }
}
