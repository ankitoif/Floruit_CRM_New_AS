namespace OIF.Cams.Model.ViewModels
{
    public class ServiceRequestDashboardModel
    {
        public long LeadId { get; set; }
        public string ProductRefNo { get; set; }
        public string CompanyName { get; set; }
        public DateTime LeadCreatedDateTime { get; set; }
        public string LeadType { get; set; }
        public string CSP_SalesName { get; set; }
        public string LeadSource { get; set; }
        public string Address { get; set; }
        public string CreatedBy { get; set; }
        public string LeadStatus { get; set; }
    }

    public class ServiceRequestDashboardViewModel
    {
        public List<ServiceRequestDashboardModel> Leads { get; set; } = new List<ServiceRequestDashboardModel>();

        public string DashboardType { get; set; }
        public bool IsAgent { get; set; }
        public int TotalCount { get; set; }
        public List<StatusCount> StatusCounts { get; set; } = new List<StatusCount>();
    }
    public class StatusCount
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }
}
