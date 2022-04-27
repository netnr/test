using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Netnr.Test.Controllers
{
    public class PathController : Controller
    {
        public IHostingEnvironment hosting;

        public PathController(IHostingEnvironment env)
        {
            hosting = env;
        }

        public IActionResult Index()
        {
            var web = "WebRootPath:" + hosting.WebRootPath;
            var content = "ContentRootPath:" + hosting.ContentRootPath;
            Console.WriteLine(web);
            Console.WriteLine(content);
            var list = new List<string>()
            {
                web,
                content
            };
            string result = string.Join(Environment.NewLine, list);

            return Content(result);
        }
    }
}