﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    // TODO : this controller must be authorize by SuperAdmin policy!
    [Authorize]
    public class AdminController : Controller
    {
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [Authorize(Policy = "SuperAdmin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}