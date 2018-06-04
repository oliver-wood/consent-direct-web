using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace Consent.Direct.Api.Models
{
    [Serializable]
    public class DataSubject
    {
        [Required]
        [EmailAddress]
        // [IgnoreDataMember]
        public string EmailAddress 
        {
            get { return _emailaddress; } 
            set {
                if (!string.IsNullOrEmpty(value)) {
                    var c = new Nethereum.Hex.HexConvertors.HexUTF8StringConvertor();
                    var h = Nethereum.Web3.Web3.Sha3(value);
                    _hash = !h.StartsWith("0x") ? $"0x{h}" : h;
                    _emailaddress = value;
                }
            } 
        }
        private string _emailaddress = string.Empty;

        [RegularExpression(pattern: @"^0x([0-9A-z]{64})$")]
        public string EmailHash { get {return _hash;} set { _hash = value; } }
        private string _hash = string.Empty;

        [Required]
        [RegularExpression(pattern: @"^0x([0-9A-z]{40})$")]
        public string Account { get; set; }

    }
}

