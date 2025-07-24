using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Model.ViewModels
{
    public class BranchViewModel
    {
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string BranchAddress { get; set; }

        public long? CustomerOrgId { get; set; }
        public IEnumerable<SelectListItem> CustomerOrgs { get; set; } = new List<SelectListItem>();

        public List<BranchListViewModel> BranchList { get; set; } = new List<BranchListViewModel>();
    }
}
