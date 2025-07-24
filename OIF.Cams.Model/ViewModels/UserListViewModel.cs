using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIF.Cams.Model.ViewModels
{
    public class UserListViewModel
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public DateOnly? DOB { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Branch { get; set; }
        public string Organisation { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsUserLockOut { get; set; }

        public string Role { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
