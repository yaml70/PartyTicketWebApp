﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace partywebapp.Controllers
{
    public class PartyImagesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}