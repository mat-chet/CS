using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthApp.Models
{
    public class AuthRequestModel
    {
        [Required(ErrorMessage = "1")]
        public string SAMLRequest { get; set; }
    }
}
