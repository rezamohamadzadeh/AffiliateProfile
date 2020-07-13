
using System.ComponentModel.DataAnnotations;

namespace AffiliateProfile.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
