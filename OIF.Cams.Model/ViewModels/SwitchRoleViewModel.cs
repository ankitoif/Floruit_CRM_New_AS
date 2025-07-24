using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Model.ViewModels
{
    public class SwitchRoleViewModel
    {
        public List<string> AssignedRoles { get; set; }
        public List<string> AvailableRoles { get; set; }
        public string CurrentActiveRole { get; set; }
    }
}
