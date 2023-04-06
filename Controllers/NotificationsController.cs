using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hoist.Data;
using Hoist.Models;
using Microsoft.AspNetCore.Authorization;
using Hoist.Services.Interfaces;

namespace Hoist.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
       
        private readonly IBTNotifications _btNotifications;

        public NotificationsController(IBTNotifications btNotifications)
        {
            _btNotifications = btNotifications;
        }


        public async Task<IActionResult> MarkViewed(int? id)
        {

            if (id != null)
            {
                await _btNotifications.MarkViewed(id);
                return RedirectToAction("Dashboard", "Home");
            }

            return View();

        }


    }
}
