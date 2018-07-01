using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace VollyV2.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public string Username { get; set; }
        
        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public string StatusMessage { get; set; }
        [Display(Name = "Profile Image")]
        public string ProfileImage { get; set; }
        [Display(Name = "Upload/Update Profile Image")]
        public IFormFile ProfileImageFile { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
    }
}
