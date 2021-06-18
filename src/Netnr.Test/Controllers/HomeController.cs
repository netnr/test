using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Netnr.Test.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ajax(int page = 1, int rows = 30, string sort = "WstartDate")
        {
            string result = string.Empty;

            result = sort == null ? "isnull" : sort;

            return Content(result);
        }

    }
}
