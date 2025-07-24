using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Model.ViewModels
{
    public class ServiceTypeViewModel
    {
        public string ServiceTypeDescription { get; set; }

        public long? CustomerOrgId { get; set; }
        public List<SelectListItem> CustomerOrgs { get; set; } = new List<SelectListItem>();

        public List<ServiceTypeListViewModel> ServiceTypeList { get; set; } = new List<ServiceTypeListViewModel>();
    }
}
