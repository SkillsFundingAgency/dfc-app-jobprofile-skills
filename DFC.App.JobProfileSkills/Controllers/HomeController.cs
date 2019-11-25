﻿using DFC.App.JobProfileSkills.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DFC.App.JobProfileSkills.Controllers
{
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}