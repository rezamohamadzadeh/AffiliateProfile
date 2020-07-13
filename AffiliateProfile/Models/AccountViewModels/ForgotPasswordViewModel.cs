
using System.ComponentModel.DataAnnotations;

namespace AffiliateProfile.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "لطفا فیلد {0} را وارد کنید")]
        [EmailAddress(ErrorMessage = "لطفا ایمیل را بدرستی وارد کنید")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }
    }
}
