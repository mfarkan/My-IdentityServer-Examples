using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Models
{
    public class LoginViewModel
    {
        public string ReturnUrl { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }
}
