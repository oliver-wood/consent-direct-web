using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Consent.Direct.Api.Models
{
    [Serializable]
    public class DataSubject
    {
        [Required]
        [RegularExpression(pattern: @"^0x([0-9A-z]{64})$")]
        public string EmailHash { get; set; }

        [Required]
        [RegularExpression(pattern: @"^0x([0-9A-z]{40})$")]
        public string Account { get; set; }
    }
}

