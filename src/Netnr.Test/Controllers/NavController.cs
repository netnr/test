using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Netnr.Test.Controllers
{
    public class NavController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}