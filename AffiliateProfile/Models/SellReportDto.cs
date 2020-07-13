using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AffiliateProfile.Models
{
    public class SellReportDto
    {
        public SellReportDto()
        {
            SkipDate = true;
        }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public FilterType FilterType { get; set; }
        public bool SkipDate { get; set; }
    }
    public enum FilterType
    {
        All,
        OnlyRegister,
        Sells
    }
}
