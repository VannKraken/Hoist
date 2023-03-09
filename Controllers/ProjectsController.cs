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
using Hoist.Models.ViewModels;
using Hoist.Models.Enums;
using System.ComponentModel.Design;


namespace Hoist.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {

        private readonly UserManager<BTUser> _userManager;
        private readonly SignInManager<BTUser> _signInManager;
        private readonly IBTFileService _btFileService;
        private readonly IBTProjectService _btProjectService;
        private readonly IBTRolesService _btRolesService;

        public ProjectsController(
                                    UserManager<BTUser> userManager,
                                    SignInManager<BTUser> signInManager,
                                    IBTFileService btFileService,
                                    IBTProjectService btProjectService,
                                    IBTRolesService btRolesService
                                 )
        {

            _userManager = userManager;
            _signInManager = signInManager;
            _btFileService = btFileService;
            _btProjectService = btProjectService;
            _btRolesService = btRolesService;
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignProjectManager(int? projectId)
        {

            if (projectId == null)
            {
                return NotFound();
            }
            int companyId = User.Identity!.GetCompanyId();



            IEnumerable<BTUser> projectManagers = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), companyId);

            BTUser? currentProjectManager = await _btProjectService.GetProjectManagerAsync(projectId);

            AssignPMViewModel viewModel = new()
            {
                Project = await _btProjectService.GetProjectAsync(projectId, companyId),
                ProjectManagerList = new SelectList(projectManagers, "Id", "FullName", currentProjectManager?.Id),
                ProjectManagerId = currentProjectManager?.Id
            };




            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignProjectManager(AssignPMViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.ProjectManagerId))
            {
                await _btProjectService.AddProjectManagerAsync(viewModel.ProjectManagerId, viewModel.Project?.Id);
                return RedirectToAction("Details", new { id = viewModel.Project?.Id });
            }

            ModelState.AddModelError("ProjectManagerId", "No Project Manager Chosen. Please select a Project Manager.");

            int companyId = User.Identity!.GetCompanyId();

            IEnumerable<BTUser> projectManagers = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), companyId);
            BTUser? currentProjectManager = await _btProjectService.GetProjectManagerAsync(viewModel.Project?.Id);


            //Reset viewmodel to reset the page.
            viewModel.Project = await _btProjectService.GetProjectAsync(viewModel.Project?.Id, companyId);
            viewModel.ProjectManagerList = new SelectList(projectManagers, "Id", "FullName", currentProjectManager?.Id);
            viewModel.ProjectManagerId = currentProjectManager?.Id;


            return View(viewModel);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AssignProjectMembers(int? projectId)
        {
            if (projectId == null)
            {
                return NotFound();
            }

            int companyId = User.Identity!.GetCompanyId();
            Project? project = await _btProjectService.GetProjectAsync(projectId, companyId);


            List<BTUser> submitters = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.Submitter), companyId);
            List<BTUser> developers = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.Developer), companyId);

            List<BTUser> userList = submitters.Concat(developers).ToList();

            List<string> currentMembers = project.Members.Select(m => m.Id).ToList();

            ProjectMembersViewModel viewModel = new()
            {
                Project = project,
                UsersList =  new MultiSelectList(userList, "Id", "FullName", currentMembers)
                
            };

            return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, ProjectManager")]
    public async Task<IActionResult> AssignProjectMembers(ProjectMembersViewModel viewModel)
    {

            int companyId = User.Identity!.GetCompanyId();

            if (viewModel.SelectedMembers != null)
            {

                //remove members

                await _btProjectService.RemoveMembersFromProjectAsync(viewModel.Project!.Id, companyId);

                //Add members

                await _btProjectService.AddMembersToProjectAsync(viewModel.SelectedMembers, viewModel.Project.Id, companyId);

                return RedirectToAction("Details", new { id = viewModel.Project!.Id});
            }

            //REset the form

            ModelState.AddModelError("SelectedMembers", "No Members Chosen. Please select Members for the project.");

            List<BTUser> submitters = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.Submitter), companyId);
            List<BTUser> developers = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.Developer), companyId);
            List<BTUser> userList = submitters.Concat(developers).ToList();

            List<string> currentMembers = viewModel.Project!.Members.Select(m => m.Id).ToList();


            return View(viewModel);
        }

    // GET: Projects
    public async Task<IActionResult> Index(int? pageNum)
    {

        int pageSize = 8;  //Number per page
        int page = pageNum ?? 1;  //Which page number clicked upon on the page.

        int companyId = User.Identity!.GetCompanyId();

        IPagedList<Project> projects = (await _btProjectService.GetProjectsAsync(companyId)).ToPagedList(page, pageSize);

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

        Project? project = await _btProjectService.GetProjectAsync(id.Value, companyId);
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
    public async Task<IActionResult> Create([Bind("Id,CompanyId,ProjectPriorityId,Name,Description,Created,StartDate,EndDate,Archived,FileData,FileType,FormFile")] Project project, IEnumerable<string> selected)
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
            if (selected != null)
            {
                await _btProjectService.AddMembersToProjectAsync(selected, project.Id, companyId);
            }

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

        IEnumerable<BTUser> developers = await _btRolesService.GetUsersInRoleAsync("Developer", companyId);

        IEnumerable<string> currentDevelopers = project.Members.Select(m => m.Id);

        ViewData["Developers"] = new MultiSelectList(developers, "Id", "FullName", currentDevelopers);
        ViewData["ProjectPriorityId"] = new SelectList(priorities, "Id", "Name");
        return View(project);
    }

    // POST: Projects/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyId,ProjectPriorityId,Name,Description,Created,StartDate,EndDate,Archived,FileData,FileType,FormFile")] Project project, IEnumerable<string> selected)
    {


        if (id != project.Id)
        {
            return NotFound();
        }
        int companyId = User.Identity!.GetCompanyId();
        ModelState.Remove("CompanyId");

        if (ModelState.IsValid)
        {

            try
            {



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

                if (selected != null)
                {
                    await _btProjectService.AddMembersToProjectAsync(selected, project.Id, companyId);
                }



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

        ViewData["Developers"] = new MultiSelectList(await _btRolesService.GetUsersInRoleAsync("Developer", companyId));
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

    public async Task<IActionResult> MyProjects()
    {
            string? userId = _userManager.GetUserId(User);

            int? companyId =  User.Identity.GetCompanyId();

            IEnumerable<Project> projects = await _btProjectService.GetUserProjectsAsync(companyId.Value, userId);
            



        return View(projects);

    }

    public async Task<IActionResult> AddMemberToProject(int projectId)
    {
        int companyId = User.Identity!.GetCompanyId();

        Project project = await _btProjectService.GetProjectAsync(projectId, companyId);

        ViewData["Developers"] = new MultiSelectList(await _btRolesService.GetUsersInRoleAsync("Developer", companyId));
        return View(project);

    }


    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> AddMemberToProject() 
    //{

    //    return View();
    //}

}
}
