using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hoist.Data;
using Hoist.Models;
using Microsoft.AspNetCore.Identity;
using Hoist.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;
using Hoist.Extensions;

namespace Hoist.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        
        private readonly UserManager<BTUser> _userManager;
        private readonly SignInManager<BTUser> _signInManager;
        private readonly IBTFileService _btFileService;
        private readonly IBTProjectService _btProjectService;

        public ProjectsController(
                                    UserManager<BTUser> userManager,
                                    SignInManager<BTUser> signInManager,
                                    IBTFileService btFileService,
                                    IBTProjectService btProjectService)
        {
            
            _userManager = userManager;
            _signInManager = signInManager;
            _btFileService = btFileService;
            _btProjectService = btProjectService;
        }

        // GET: Projects
        public async Task<IActionResult> Index(int? pageNum)
        {

            int pageSize = 8;  //Number per page
            int page = pageNum ?? 1;  //Which page number clicked upon on the page.

            int companyId = User.Identity!.GetCompanyId();

            IPagedList<Project> projects =  (await _btProjectService.GetProjectsAsync(companyId)).ToPagedList(page, pageSize);

            return View(projects);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity!.GetCompanyId();

            Project? project = await _btProjectService.GetProjectAsync(id.Value,companyId);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public async Task<IActionResult> Create()
        {
            IEnumerable<ProjectPriority> priorities = await _btProjectService.GetProjectPrioritiesAsync();
            
            
            ViewData["ProjectPriorityId"] = new SelectList(priorities, "Id", "Name");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CompanyId,ProjectPriorityId,Name,Description,Created,StartDate,EndDate,Archived,FileData,FileType,FormFile")] Project project)
        {
            ModelState.Remove("CompanyId");

            if (ModelState.IsValid)
            {
                int companyId = User.Identity!.GetCompanyId();


                project.CompanyId = companyId;


                //TODO Created Date and date for post gres
                project.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);
                project.StartDate = DataUtility.GetPostGresDate(project.StartDate);
                project.EndDate = DataUtility.GetPostGresDate(project.EndDate);

                //Archived
                project.Archived = false;


                //TODO File conversion
                if (project.FormFile != null)
                {
                    project.FileData = await _btFileService.ConvertFileToByteArrayAsync(project.FormFile);
                    project.FileType = project.FormFile.ContentType;
                }

               await _btProjectService.AddProjectAsync(project);

                return RedirectToAction(nameof(Index));
            }

            IEnumerable<ProjectPriority> priorities = await _btProjectService.GetProjectPrioritiesAsync();

            ViewData["ProjectPriorityId"] = new SelectList(priorities, "Id", "Name", project.ProjectPriorityId);
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int companyId = User.Identity!.GetCompanyId();

            Project? project = await _btProjectService.GetProjectAsync(id.Value, companyId);
            if (project == null)
            {
                return NotFound();
            }


            IEnumerable<ProjectPriority> priorities = await _btProjectService.GetProjectPrioritiesAsync();


            ViewData["ProjectPriorityId"] = new SelectList(priorities, "Id", "Name");
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyId,ProjectPriorityId,Name,Description,Created,StartDate,EndDate,Archived,FileData,FileType,FormFile")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            ModelState.Remove("CompanyId");

            if (ModelState.IsValid)
            {
                try
                {

                    int companyId = User.Identity!.GetCompanyId();


                    project.CompanyId = companyId;


                    //TODO Created Date and date for post gres
                    project.Created = DataUtility.GetPostGresDate(project.Created);
                    project.StartDate = DataUtility.GetPostGresDate(project.StartDate);
                    project.EndDate = DataUtility.GetPostGresDate(project.EndDate);

                    //Archived
                    project.Archived = false;


                    //TODO File conversion
                    if (project.FormFile != null)
                    {
                        project.FileData = await _btFileService.ConvertFileToByteArrayAsync(project.FormFile);
                        project.FileType = project.FormFile.ContentType;
                    }

                    _btProjectService.UpdateProjectAsync(project);
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_btProjectService.ProjectExists(project.Id))
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

            IEnumerable<ProjectPriority> priorities = await _btProjectService.GetProjectPrioritiesAsync();

            ViewData["ProjectPriorityId"] = new SelectList(priorities, "Id", "Name", project.ProjectPriorityId);
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int id)
        {

            try
            {
                int companyId = User.Identity!.GetCompanyId();

                Project project = await _btProjectService.GetProjectAsync(id, companyId);

                await _btProjectService.ArchiveProjectAsync(project);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                throw;
            }            

        }

    }
}
