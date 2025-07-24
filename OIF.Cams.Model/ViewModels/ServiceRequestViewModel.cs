using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace OIF.Cams.Model.ViewModels
{
    public class ServiceRequestViewModel
    {
        [Required]
        public string PolicyNumber { get; set; }

        [Required]
        public string PolicyHolderName { get; set; }

        public long? CrmId { get; set; }

        public long? ServiceTypeId { get; set; }

        public List<SelectListItem> CrmList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ServiceTypeList { get; set; } = new List<SelectListItem>();

        public Dictionary<long, List<ServiceTypeDocumentDto>> ServiceTypeDocuments { get; set; } = new();
    }

    public class ServiceTypeDocumentDto
    {
        public long ServiceTypeDocumentId { get; set; }
        public string DocumentCategory { get; set; } 
    }
}
