﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tupencauymobile.Models
{
    public class AuthReturn
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public string TenantId { get; set; }
        public string IdUser { get; set; }
    }
}
