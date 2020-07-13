using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace AffiliateProfile.Models
{
    public class SupportDto
    {
        public int Id { get; set; }

        [Display(Name = "SupportType")]
        [Required(ErrorMessage = "Please Enter {0}")]
        public int TypeId { get; set; }

        [Display(Name = "SupportType")]
        public string SupportType { get; set; }

        [Display(Name = "Message")]
        [Required(ErrorMessage = "Please Enter {0}")]
        public string Message { get; set; }

        [Display(Name = "SendDateTime")]
        public DateTime SendDateTime { get; set; }

        public string SenderUserName { get; set; }
        
        public string SenderUserId { get; set; }

        [Display(Name = "AnswerMessage")]
        public string AnswerMessage { get; set; }

        [Display(Name = "AnswerDateTime")]
        public DateTime? AnswerDateTime { get; set; }

        [Display(Name = "AnswerUserType")]
        public string AnswerUserName { get; set; }

        public string AnswerUerId { get; set; }

        [Display(Name = "SupportPosition")]
        public string SupportPosition { get; set; }

        [Display(Name = "File")]
        public string File { get; set; }

        public string Email { get; set; }

        [Display(Name = "Attachment")]
        public IFormFile Attachment { get; set; }


    }
}
