using System;
using Microsoft.AspNetCore.Mvc;

using ApplianceRemote.Models;

namespace ApplianceRemote.Controllers
{
    [Route("api/[controller]")]
    public class ApplianceController : Controller
    {
        private static bool _isOn = false;

        [HttpGet]
        public IActionResult GetStatus()
        {

            return Json(new
            {
                isOn = _isOn
            });
        }

        [HttpPost]
        [Route("turnon")]
        public IActionResult TurnOn()
        {
            _isOn = true;
            return Ok();
        }

        [HttpPost]
        [Route("turnoff")]
        public IActionResult TurnOff()
        {
            _isOn = false;
            return Ok();
        }
    }
}
