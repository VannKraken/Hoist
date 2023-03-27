using Hoist.Extensions;
using Hoist.Models;
using Hoist.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Hoist.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        #region Dependency Injection
        private readonly ILogger<HomeController> _logger;
        private readonly IBTCompanyService _btCompanyService;
        private readonly UserManager<BTUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IBTCompanyService btCompanyService, UserManager<BTUser> userManager)
        {
            _logger = logger;
            _btCompanyService = btCompanyService;
            _userManager = userManager;
        } 
        #endregion


        #region Main Views
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Dashboard()
        {

            int companyId = User.Identity!.GetCompanyId();
            string? userId = _userManager.GetUserId(User);

            Company company = await _btCompanyService.GetEverythingForCompanyAsync(companyId);

            ViewBag.Notificatons = await _btCompanyService.GetMemberNotifications(userId, companyId);



            return View(company);
        }
        #endregion


        #region Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        } 
        #endregion
    }
}