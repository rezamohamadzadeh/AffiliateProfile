
namespace AffiliateProfile.Models
{
    public class DataTableModel
    {
        public string draw { get; set; }
        public string length { get; set; }
        public string start { get; set; }
        public int recordsTotal { get; set; } = 0;
        public int pageSize { get; set; }
        public int skip { get; set; }
        public string sortColumn { get; set; }
        public string sortColumnDirection { get; set; }
        public string searchValue { get; set; }
    }
}
