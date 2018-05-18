using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV2.Models.Volly
{
    public class SuggestionModel
    {
        public string Name { get; set; }
        [Required]
        public string Message { get; set; }

        public string GetEmailMessage()
        {
            return "<p>Suggestion: " + Message + "<p/>";
        }
    }
}
