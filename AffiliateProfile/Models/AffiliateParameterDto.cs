using System.ComponentModel.DataAnnotations;

namespace AffiliateProfile.Models
{
    public class AffiliateParameterDto : BaseDto
    {
        [Display(Name = "Param1")]
        [Required(ErrorMessage = "Please enter {0}")]
        public string Param1 { get; set; }

        [Display(Name = "Param1Val")]
        [Required(ErrorMessage = "Please enter {0}")]
        public string Param1Val { get; set; }

        [Display(Name = "Param2")]
        public string Param2 { get; set; }

        [Display(Name = "Param2Val")]
        public string Param2Val { get; set; }

        [Display(Name = "Param3")]
        public string Param3 { get; set; }

        [Display(Name = "Param3Val")]
        public string Param3Val { get; set; }

        [Display(Name = "Param4")]
        public string Param4 { get; set; }

        [Display(Name = "Param4Val")]
        public string Param4Val { get; set; }

        public string UserId { get; set; }
    }
}
