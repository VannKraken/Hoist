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
using Hoist.Extensions;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;

namespace Hoist.Controllers
{
    [Authorize(Roles="Admin")]
    public class InvitesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTProjectService _btProjectService;
        private readonly IBTCompanyService _bTCompanyService;
        private readonly IBTInviteService _btInviteService;
        private readonly IEmailSender _emailService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IDataProtector _dataProtector;
        private readonly IConfiguration _configuration;

        public InvitesController(ApplicationDbContext context, IBTProjectService btProjectService, IBTCompanyService bTCompanyService,
                                IBTInviteService btInviteService, IEmailSender emailService, UserManager<BTUser> userManager, IDataProtectionProvider dataProtectorProvider, IConfiguration configuration)
        {
            _context = context;
            _btProjectService = btProjectService;
            _bTCompanyService = bTCompanyService;
            _btInviteService = btInviteService;
            _emailService = emailService;
            _userManager = userManager;
            _configuration = configuration;
            _dataProtector = dataProtectorProvider.CreateProtector(configuration.GetValue<string>("ProtectKey") ?? Environment.GetEnvironmentVariable("ProtectKey")!); //Creating a data protector for our
        }

        //// GET: Invites
        //public async Task<IActionResult> Index()
        //{
        //    var applicationDbContext = _context.Invites.Include(i => i.Company).Include(i => i.Invitee).Include(i => i.Invitor).Include(i => i.Project);
        //    return View(await applicationDbContext.ToListAsync());
        //}

        //// GET: Invites/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.Invites == null)
        //    {
        //        return NotFound();
        //    }

        //    var invite = await _context.Invites
        //        .Include(i => i.Company)
        //        .Include(i => i.Invitee)
        //        .Include(i => i.Invitor)
        //        .Include(i => i.Project)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (invite == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(invite);
        //}

        // GET: Invites/Create
        public async Task<IActionResult> Create()
        {

            int? companyId = User.Identity!.GetCompanyId();
            
            ViewData["ProjectId"] = new SelectList(await _btProjectService.GetProjectsAsync(companyId.Value), "Id", "Name");
            return View();
        }

        // POST: Invites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,InviteeEmail,InviteeFirstName,InviteeLastName,Message")] Invite invite)
        {
            int companyId = User.Identity!.GetCompanyId();

            ModelState.Remove("InvitorId");
            
            if (ModelState.IsValid)
            {
                try
                {
                    Guid guid = Guid.NewGuid();

                    string? token = _dataProtector.Protect(guid.ToString());
                    string? email = _dataProtector.Protect(invite.InviteeEmail!);
                    string? company = _dataProtector.Protect(companyId.ToString());

                    string? callbackUrl = Url.Action("ProcessInvite", "Invites", new { token, email, company}, protocol: Request.Scheme);

                    string body = $@"{invite.Message} <br />
                       Please join my Company. <br />
                       Click the following link to join our team. <br />
                       <a href=""{callbackUrl}"">COLLABORATE</a>";

                    string? destination = invite.InviteeEmail;
                    Company companyInstance = await _bTCompanyService.GetEverythingForCompanyAsync(companyId);

                    string? subject = $"Hoist: {companyInstance.Name} invites you to raise the flag.";

                    await _emailService.SendEmailAsync(destination!, subject, body);

                    //Save invite to db

                    invite.CompanyToken = guid;
                    invite.CompanyId = companyId;
                    invite.InviteDate = DataUtility.GetPostGresDate(DateTime.Now);
                    invite.InvitorId = _userManager.GetUserId(User);
                    invite.IsValid = true;

                    await _btInviteService.AddNewInviteAsync(invite);

                
                    return RedirectToAction(nameof(Index), "Home");

                    //TODO: SWAL for invite sent
                }
                catch (Exception)
                {

                    throw;
                }
                
            }


            ViewData["ProjectId"] = new SelectList(await _btProjectService.GetProjectsAsync(companyId), "Id", "Name");
            return View(invite);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ProcessInvite(string token, string email, string company)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(company))
            {
                return NotFound();
            }

            Guid companyToken = Guid.Parse(_dataProtector.Unprotect(token));
            string inviteeEmail = _dataProtector.Unprotect(email);
            int companyId = int.Parse(_dataProtector.Unprotect(company));

            try
            {
                Invite? invite = await _btInviteService.GetInviteAsync(companyToken, inviteeEmail, companyId);

                if (invite != null)
                {
                    return View(invite);
                }

                return NotFound();
            }
            catch (Exception)
            {

                throw;
            }


        }


    }
}
