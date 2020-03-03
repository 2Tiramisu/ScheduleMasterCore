﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hos.ScheduleMaster.QuartzHost.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.Core.Log;
using Microsoft.AspNetCore.Authorization;

namespace Hos.ScheduleMaster.QuartzHost.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class QuartzController : ControllerBase
    {
        private readonly ILogger<QuartzController> _logger;
        private SmDbContext _db;

        public QuartzController(ILogger<QuartzController> logger, SmDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Start([FromQuery]Guid sid)
        {
            bool success = await QuartzManager.StartWithRetry(sid);
            if (success) return Ok();
            return BadRequest();
        }


        [HttpPost]
        public async Task<IActionResult> Stop([FromQuery]Guid sid)
        {
            bool success = await QuartzManager.Stop(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Pause([FromQuery]Guid sid)
        {
            bool success = await QuartzManager.Pause(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Resume([FromQuery]Guid sid)
        {
            bool success = await QuartzManager.Resume(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> RunOnce([FromQuery]Guid sid)
        {
            bool success = await QuartzManager.RunOnce(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Init()
        {
            try
            {
                await QuartzManager.InitScheduler();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Shutdown()
        {
            try
            {
                await QuartzManager.Shutdown(false);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        //[HttpGet]
        //public IActionResult HealthCheck()
        //{
        //    return Ok("i am ok");
        //}

        [HttpGet, AllowAnonymous]
        public IActionResult Get()
        {
            //var sourcePath = "https://localhost:44301/static/downloadpluginfile?pluginname=test";
            //var zipPath = $"{Directory.GetCurrentDirectory()}\\Plugins\\test.zip";
            //var pluginPath = $"{Directory.GetCurrentDirectory()}\\Plugins\\test";
            //WebClient client = new WebClient();
            //client.DownloadFile(new Uri(sourcePath), zipPath);
            ////将指定 zip 存档中的所有文件都解压缩到文件系统的一个目录下
            //ZipFile.ExtractToDirectory(zipPath, pluginPath, true);
            //System.IO.File.Delete(zipPath);
            return Ok("5555555555555555");
            //Common.QuartzManager.StartWithRetry(new Core.Models.ScheduleView
            //{
            //    Schedule = new Core.Models.ScheduleEntity
            //    {
            //        AssemblyName = "Hos.ScheduleMaster.Demo",
            //        ClassName = "Hos.ScheduleMaster.Demo.Simple",
            //        CreateTime = DateTime.Now,
            //        CreateUserId = 1,
            //        CreateUserName = "hh",
            //        CronExpression = "0/1 * * * * ?",
            //        Id = Guid.NewGuid(),
            //        RunMoreTimes = true,
            //        StartDate = DateTime.Now.AddMinutes(-3),
            //        Status = 0,
            //        Title = "测试任务"
            //    }
            //}, (sid, time) => { });
        }
    }
}
