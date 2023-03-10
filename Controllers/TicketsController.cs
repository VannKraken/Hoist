using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hoist.Data;
using Hoist.Models;
using Hoist.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Hoist.Models.Enums;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Hoist.Services;
using Hoist.Extensions;
using Hoist.Models.ViewModels;
using System.ComponentModel.Design;

namespace Hoist.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {

        private readonly UserManager<BTUser> _userManager;
        private readonly SignInManager<BTUser> _signInManager;
        private readonly IBTFileService _btFileService;
        private readonly IBTTicketService _btTicketService;
        private readonly IBTProjectService _btProjectService;
        private readonly IBTRolesService _btRolesService;

        public TicketsController(UserManager<BTUser> userManager, SignInManager<BTUser> signInManager, IBTFileService btFileService, IBTTicketService btTicketService, IBTProjectService btProjectService, IBTRolesService btRolesService)
        {
            
            _userManager = userManager;
            _signInManager = signInManager;
            _btFileService = btFileService;
            _btTicketService = btTicketService;
            _btProjectService = btProjectService;
            _btRolesService = btRolesService;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            int companyId = User.Identity!.GetCompanyId();

            string? userId = _userManager.GetUserId(User);

            BTUser? btUser = await _userManager.GetUserAsync(User);

            IEnumerable<Ticket> tickets = await _btTicketService.GetCompanyTicketsAsync(companyId);

            return View(tickets);
        }



        //Tabulator
        //public IActionResult TicketData()
        //{
        //    var data = _context.Tickets.Include(t => t.DeveloperUser)
        //                               .Include(t => t.Project)
        //                               .Include(t => t.SubmitterUser)
        //                               .Include(t => t.TicketPriority)
        //                               .Include(t => t.TicketStatus)
        //                               .Include(t => t.TicketType)
        //                               .Select(t => new
        //                               {
        //                                   title = t.Title,
        //                                   description = t.Description.Truncate(40),
        //                                   submitter = t.SubmitterUser!.FullName,
        //                                   developer = t.DeveloperUser!.FullName,
        //                                   type = t.TicketType.Name,
        //                                   status = t.TicketStatus.Name,
        //                                   priority = t.TicketPriority.Name,
        //                                   created = t.Created.ToString("yyyy/MM/dd"),
        //                                   id = t.Id.ToString()
        //                               }).ToList();

        //    return new JsonResult(data);
        //}

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket? ticket = await _btTicketService.GetTicketAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create()
        {

            int companyId = User.Identity!.GetCompanyId();
            BTUser? btUser = await _userManager.GetUserAsync(User);

            IEnumerable<Project> projects = await _btProjectService.GetProjectsAsync(companyId);
            IEnumerable<TicketPriority> priorities = await _btTicketService.GetTicketPriorities();


            ViewData["ProjectId"] = new SelectList(projects, "Id", "Name");
            ViewData["TicketPriorityId"] = priorities;
            ViewData["TicketStatusId"] = new SelectList(await _btTicketService.GetTicketStatuses(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _btTicketService.GetTicketTypes(), "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,TicketTypeId,TicketStatusId,TicketPriorityId,DeveloperUserId,SubmitterUserId,Title,Description,Created,Archived,ArchivedByProject")] Ticket ticket)
        {
            ModelState.Remove("SubmitterUserId");
            ModelState.Remove("TicketStatusId");

            if (ModelState.IsValid)
            {
                try
                {
                    string? userId = _userManager.GetUserId(User);

                    ticket.SubmitterUserId = userId;
                    ticket.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);
                    ticket.TicketStatusId = (await _btTicketService.GetTicketStatuses()).FirstOrDefault(s => s.Name == "New")!.Id; 
                        

                    ticket.Archived = false;
                    ticket.ArchivedByProject = false;

                    await _btTicketService.AddTicketAsync(ticket);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {

                    throw;
                }


                //ticket.ProjectId = _context.Companies.Include(c => c.Projects).ThenInclude(p => p.Members.Where(m => m.Id == userId)


            }

            int companyId = User.Identity!.GetCompanyId();

            IEnumerable<Project> projects = await _btProjectService.GetProjectsAsync(companyId);
            IEnumerable<TicketPriority> priorities = await _btTicketService.GetTicketPriorities();

            ViewData["ProjectId"] = new SelectList(projects, "Id", "Name");
            ViewData["TicketPriorityId"] = priorities;
            ViewData["TicketStatusId"] = new SelectList(await _btTicketService.GetTicketStatuses(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _btTicketService.GetTicketTypes(), "Id", "Name");
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket? ticket = await _btTicketService.GetTicketAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }


            BTUser? btUser = await _userManager.GetUserAsync(User);
            int companyId =  btUser!.CompanyId;

            IEnumerable<Project> projects = await _btProjectService.GetProjectsAsync(companyId);

            IEnumerable<BTUser> developers = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.Developer), companyId);

            IEnumerable<TicketPriority> priorities = await _btTicketService.GetTicketPriorities();
            IEnumerable<TicketStatus> statuses = await _btTicketService.GetTicketStatuses();

            ViewData["DeveloperUserId"] = new SelectList(developers, "Id", "FullName", ticket.DeveloperUserId);
            ViewData["ProjectId"] = new SelectList(projects, "Id", "Name");
            ViewData["TicketPriorityId"] = priorities;
            ViewData["TicketStatusId"] = statuses;
            ViewData["TicketTypeId"] = new SelectList(await _btTicketService.GetTicketTypes(), "Id", "Name");



            
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,TicketTypeId,TicketStatusId,TicketPriorityId,DeveloperUserId,SubmitterUserId,Title,Description,Created,Updated,Archived,ArchivedByProject")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    ticket.Created = DataUtility.GetPostGresDate(ticket.Created);
                    ticket.Updated = DataUtility.GetPostGresDate(DateTime.UtcNow);


                    await _btTicketService.UpdateTicketAsync(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _btTicketService.TicketExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            BTUser? btUser = await _userManager.GetUserAsync(User);

            int companyId = User.Identity!.GetCompanyId();

            IEnumerable<Project> projects = await _btProjectService.GetProjectsAsync(companyId);
            IEnumerable<BTUser> developers = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.Developer), companyId);
            IEnumerable<TicketPriority> priorities = await _btTicketService.GetTicketPriorities();
            IEnumerable<TicketStatus> statuses = await _btTicketService.GetTicketStatuses();


            ViewData["DeveloperUserId"] = new SelectList(developers, "Id", "FullName", ticket.DeveloperUserId);
            ViewData["ProjectId"] = new SelectList(projects, "Id", "Name");
            ViewData["TicketPriorityId"] = priorities;
            ViewData["TicketStatusId"] = statuses;
            ViewData["TicketTypeId"] = new SelectList(await _btTicketService.GetTicketTypes(), "Id", "Name");
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket? ticket = await _btTicketService.GetTicketAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {

            Ticket? ticket = await _btTicketService.GetTicketAsync(id);
            await _btTicketService.ArchiveTicketAsync(ticket);
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketComment([Bind("Id,TicketId,BTUserId,Comment,Created")]TicketComment ticketComment)
        {

            int ticketId = ticketComment.TicketId;

            ModelState.Remove("BTUserId");
            if (ModelState.IsValid)
            {

               

                ticketComment.BTUserId = _userManager.GetUserId(User);

                ticketComment.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);

                await _btTicketService.AddTicketComment(ticketComment);

                return RedirectToAction("Details", new { id = ticketId });
            }

            return RedirectToAction("Details", new { id = ticketId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketAttachment([Bind("Id,FormFile,Description,TicketId")] TicketAttachment ticketAttachment)
        {
            string statusMessage;
            ModelState.Remove("BTUserId");

            if (ModelState.IsValid && ticketAttachment.FormFile != null)
            {
                ticketAttachment.FileData = await _btFileService.ConvertFileToByteArrayAsync(ticketAttachment.FormFile);                
                ticketAttachment.FileType = ticketAttachment.FormFile.ContentType;

                ticketAttachment.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);
                ticketAttachment.BTUserId = _userManager.GetUserId(User);

                await _btTicketService.AddTicketAttachmentAsync(ticketAttachment);
                statusMessage = "Success: New attachment added to Ticket.";
            }
            else
            {
                statusMessage = "Error: Invalid data.";

            }

            return RedirectToAction("Details", new { id = ticketAttachment.TicketId, message = statusMessage });
        }

        [HttpGet]
        public async Task<IActionResult> MyTickets()
        {

            string? userId = _userManager.GetUserId(User);

            int? companyId = User.Identity.GetCompanyId();

            IEnumerable<Ticket> tickets = await _btTicketService.GetUserTicketsAsync(userId, companyId);

            return View(tickets);

        }

       



        public async Task<IActionResult> AssignTicketDeveloper(int? ticketId)
        {

            if(ticketId == null)
            {
                return NotFound();
            }

            int companyId = User.Identity!.GetCompanyId();

            Ticket ticket = await _btTicketService.GetTicketAsync(ticketId);

            List<BTUser> developers = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.Developer), companyId);

            string? currentDeveloperId = ticket.DeveloperUserId;

            AssignDeveloperViewModel viewModel = new()
            {
                
                Ticket = ticket,
                DeveloperList = new SelectList(developers, "Id", "FullName", currentDeveloperId)

            };





            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTicketDeveloper(AssignDeveloperViewModel viewModel)
        {

            Ticket? ticket = await _btTicketService.GetTicketAsync(viewModel.Ticket?.Id);

            if (!string.IsNullOrEmpty(viewModel.DeveloperId))
            {

                ticket.DeveloperUserId = viewModel.DeveloperId;
                await _btTicketService.UpdateTicketAsync(ticket);
            return RedirectToAction("Details", new { id = ticket.Id });
            }

            ModelState.AddModelError("DeveloperId", "No Developer Chosen. Please select a developer for the ticket.");

            return View(viewModel);

        }



    }
}
