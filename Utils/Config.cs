﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Owin_Auth.Utils
{
    public class Config
    {
        public static IConfiguration Configuration { get; set; }
    }
}
