using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV2.Models.Volly
{
    public class EmailDomainValidationAttribute : ValidationAttribute
    {
        private readonly List<string> _validDomains = new List<string>()
        {
            "@criticalcontrol.com"
        };


        public override bool IsValid(object value)
        {
            if (value == null) { return false; }
            string emailValue = (string)value;

            if (_validDomains.Any(emailValue.Contains))
            {
                return true;
            }

            return false;
        }
    }
}
