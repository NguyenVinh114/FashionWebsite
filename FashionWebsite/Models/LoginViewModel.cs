using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FashionWebsite.Models
{
    public class LoginViewModel
    {
        public string TenDN { get; set; }
        public string MatKhau { get; set; }
        public bool RememberMe { get; set; }
    }

}