using Microsoft.AspNetCore.Mvc;
using Netnr.AuthFailed.Models;
using Netnr.AuthFailed.Services;
using System.Diagnostics;

namespace Netnr.AuthFailed.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginPost([FromForm] string userAccount, [FromForm] string userPassword)
        {
            var model = new BaseUser
            {
                UserId = 1,
                UserAccount = "netnr",
                UserNickname = "netnr",
                CreateTime = DateTime.Now,
                UserPassword = "123456"
            };

            if (userAccount == model.UserAccount && model.UserPassword == userPassword)
            {
                var token = await IdentityService.Set(HttpContext, model);
                return Content(token);
            }

            return BadRequest("400 Incorrect username or password");
        }

        [HttpGet]
        public IActionResult AuthorizationInfoGet()
        {
            var authBase = IdentityService.Get(HttpContext);
            if (authBase == null)
            {
                return Unauthorized("401 ╯﹏╰");
            }
            return Json(authBase);
        }

        [HttpGet]
        public IActionResult ConsumeCPU(int cpu)
        {
            if (cpu > 0 && cpu <= 100)
            {
                CPUNumber = cpu;

                if (CPUStart != 1)
                {
                    CPUStart = 1;
                    CPUInit();
                }
            }
            else
            {
                CPUStart = 0;
                CPUNumber = 20;
            }

            return Ok($"CPU {cpu}%");
        }

        public static int CPUNumber { get; set; } = 20;
        public static int CPUStart { get; set; } = 0;
        public void CPUInit()
        {
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                var thread = new Thread(() =>
                {
                    var watch = Stopwatch.StartNew();
                    while (CPUStart == 1)
                    {
                        if (watch.ElapsedMilliseconds > CPUNumber)
                        {
                            Thread.Sleep(100 - CPUNumber);
                            watch.Restart();
                        }
                    }
                })
                {
                    IsBackground = true
                };
                thread.Start();
                GC.KeepAlive(thread);
            }
        }
    }
}
