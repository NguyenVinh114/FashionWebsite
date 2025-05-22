using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FashionWebsite.Models
{
    public class LoginAttempt
    {
        public int FailedCount { get; set; } = 0;
        public DateTime? LockedUntil { get; set; }
    }
}