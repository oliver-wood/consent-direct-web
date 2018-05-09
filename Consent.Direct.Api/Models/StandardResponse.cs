using System;
using System.Collections.Generic;
using System.Linq;

namespace Consent.Direct.Api.Models
{
    public class StandardResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public string JSONContent { get; set; }
    }

}