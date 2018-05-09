using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Consent.Direct.Api.Models
{
    [Serializable]
    public class DataSubjectRegistration
    {
        public string Token { get; set; }
        public DataSubject Subject { get; set; }

    }
}