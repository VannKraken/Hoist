﻿using System;
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
                UsersList = new MultiSelectList(userList, "Id", "FullName", currentMembers)

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

                return RedirectToAction("Details", new { id = viewModel.Project!.Id });
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
        public async Task<IActionResult> Index(int? pageNum, string? sortType)
        {
            int pageSize = 8;  //Number per page
            int page = pageNum ?? 1;  //Which page number clicked upon on the page.

            int companyId = User.Identity!.GetCompanyId();

            List<string> sortingTypes = new()
            {
                "A - Z",
                "Z - A",
                "Deadline, Soonest",
                "Deadline, Furthest",
                "Priority, Low",
                "Priority, High"
                
            };


            async Task<IPagedList<Project>> GetSortedProjects(string? sortString)
            {
                return sortString switch
                {
                    "A - Z" => (await _btProjectService.GetProjectsAsync(companyId)).OrderBy(p => p.Name).ToPagedList(page, pageSize),
                    "Z - A" => (await _btProjectService.GetProjectsAsync(companyId)).OrderByDescending(p => p.Name).ToPagedList(page, pageSize),
                    "Deadline, Soonest" => (await _btProjectService.GetProjectsAsync(companyId)).OrderBy(p => p.EndDate).ToPagedList(page, pageSize),
                    "Deadline, Furthest" => (await _btProjectService.GetProjectsAsync(companyId)).OrderByDescending(p => p.EndDate).ToPagedList(page, pageSize),
                    "Priority, Low" => (await _btProjectService.GetProjectsAsync(companyId)).OrderBy(p => p.ProjectPriorityId).ToPagedList(page, pageSize),
                    "Priority, High" => (await _btProjectService.GetProjectsAsync(companyId)).OrderByDescending(p => p.ProjectPriorityId).ToPagedList(page, pageSize),
                    _ => (await _btProjectService.GetProjectsAsync(companyId)).ToPagedList(page, pageSize),
                };
            }

            IPagedList<Project> sortedProjects = await GetSortedProjects(sortType);
            ViewData["SortTypes"] = new SelectList(sortingTypes, sortType);
            ViewData["CurrentSortType"] = sortType;


            


            return View(sortedProjects);
        }
        public async Task<IActionResult> MyProjects(int? pageNum, string? sortType)
        {
            int pageSize = 8;  //Number per page
            int page = pageNum ?? 1;  //Which page number clicked upon on the page.

            int companyId = User.Identity!.GetCompanyId();
            string? userId = _userManager.GetUserId(User);

            List<string> sortingTypes = new()
            {
                "A - Z",
                "Z - A",
                "Priority, High",
                "Priority, Low",

            };


            if (string.IsNullOrEmpty(sortType))
            {
                IPagedList<Project> projectsDefault = (await _btProjectService.GetUserProjectsListAsync(companyId, userId)).ToPagedList(page, pageSize);

                ViewData["SortTypes"] = new SelectList(sortingTypes, sortType);


                return View(projectsDefault);
            }

            if (sortType == "A - Z")
            {
                IPagedList<Project> projectsAlphabetical = (await _btProjectService.GetUserProjectsListAsync(companyId, userId)).OrderBy(p => p.Name).ToPagedList(page, pageSize);

                ViewData["SortTypes"] = new SelectList(sortingTypes, sortType);
                ViewData["CurrentSortType"] = sortType;

                return View(projectsAlphabetical);
            }
            if (sortType == "Z - A")
            {
                IPagedList<Project> projectsAlphabeticalRev = (await _btProjectService.GetUserProjectsListAsync(companyId, userId)).OrderByDescending(p => p.Name).ToPagedList(page, pageSize);

                ViewData["SortTypes"] = new SelectList(sortingTypes, sortType);
                ViewData["CurrentSortType"] = sortType;

                return View(projectsAlphabeticalRev);
            }
            if (sortType == "Priority, High")
            {
                IPagedList<Project> projectsDeadline = (await _btProjectService.GetUserProjectsListAsync(companyId, userId)).OrderBy(p => p.EndDate).ToPagedList(page, pageSize);

                ViewData["SortTypes"] = new SelectList(sortingTypes, sortType);
                ViewData["CurrentSortType"] = sortType;

                return View(projectsDeadline);
            }
            if (sortType == "Priority, Low")
            {
                IPagedList<Project> projectsDeadlineRev = (await _btProjectService.GetUserProjectsListAsync(companyId, userId)).OrderByDescending(p => p.EndDate).ToPagedList(page, pageSize);

                ViewData["SortTypes"] = new SelectList(sortingTypes, sortType);
                ViewData["CurrentSortType"] = sortType;

                return View(projectsDeadlineRev);
            }




            IPagedList<Project> projects = (await _btProjectService.GetUserProjectsListAsync(companyId, userId)).ToPagedList(page, pageSize);

            ViewData["SortTypes"] = new SelectList(sortingTypes, sortType);


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

            List<BTUser> submitters = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.Submitter), companyId);
            List<BTUser> developers = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.Developer), companyId);
            List<BTUser> userList = submitters.Concat(developers).ToList();
            IEnumerable<BTUser> projectManagers = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), companyId);

            ViewData["ProjectManagersId"] = new SelectList(projectManagers, "Id", "FullName");
            ViewData["MembersId"] = userList;
            return View(project);
        }

        // GET: Projects/Create
        public async Task<IActionResult> Create()
        {
            int companyId = User.Identity!.GetCompanyId();

            IEnumerable<ProjectPriority> priorities = (await _btProjectService.GetProjectPrioritiesAsync()).OrderBy(p => p.Id);
            List<BTUser> submitters = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.Submitter), companyId);
            List<BTUser> developers = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.Developer), companyId);
            List<BTUser> userList = submitters.Concat(developers).ToList();
            IEnumerable<BTUser> projectManagers = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), companyId);


            ViewData["ProjectPriorityId"] = new SelectList(priorities, "Id", "Name");
            ViewData["MembersId"] = new MultiSelectList(userList, "Id", "FullName");

            ViewData["ProjectManagersId"] = new SelectList(projectManagers, "Id", "FullName");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CompanyId,ProjectPriorityId,Name,Description,Created,StartDate,EndDate,Archived,FileData,FileType,FormFile")] Project project, IEnumerable<string> selectedMembers, string? projectManagerId)
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

                if (projectManagerId != null)
                {
                    await _btProjectService.AddProjectManagerAsync(projectManagerId, project.Id);
                }

                if (selectedMembers != null)
                {
                    await _btProjectService.AddMembersToProjectAsync(selectedMembers, project.Id, companyId);
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

            IEnumerable<ProjectPriority> priorities = (await _btProjectService.GetProjectPrioritiesAsync()).OrderBy(p => p.Id);

            List<BTUser> submitters = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.Submitter), companyId);
            List<BTUser> developers = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.Developer), companyId);
            List<BTUser> userList = submitters.Concat(developers).ToList();
            IEnumerable<BTUser> projectManagers = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), companyId);


            ProjectPriority currentPriority = project.ProjectPriority;
            BTUser? currentProjectManager = await _btProjectService.GetProjectManagerAsync(project.Id);
            IEnumerable<string> currentDevelopers = project.Members.Select(m => m.Id);


            ViewData["ProjectPriorityId"] = new SelectList(priorities, "Id", "Name", currentPriority);
            ViewData["MembersId"] = new MultiSelectList(userList, "Id", "FullName",currentDevelopers);

            if (currentProjectManager != null) {
            ViewData["ProjectManagersId"] = new SelectList(projectManagers, "Id", "FullName", currentProjectManager.Id);
            }

            ViewData["StartDate"] = project.StartDate;
            ViewData["EndDate"] = project.EndDate;







            ViewData["Developers"] = new MultiSelectList(userList, "Id", "FullName", currentDevelopers);

            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyId,ProjectPriorityId,Name,Description,Created,StartDate,EndDate,Archived,FileData,FileType,FormFile")] Project project, IEnumerable<string> selectedMembers, string? projectManagerId)
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

                    await _btProjectService.RemoveMembersFromProjectAsync(project.Id, companyId);
                    await _btProjectService.RemoveProjectManagerAsync(project.Id);

                    if (selectedMembers != null)
                    {
                        
                        await _btProjectService.AddMembersToProjectAsync(selectedMembers, project.Id, companyId);
                    }
                    if (projectManagerId != null)
                    {
                        
                        await _btProjectService.AddProjectManagerAsync(projectManagerId, project.Id);
                    }

                    await _btProjectService.UpdateProjectAsync(project);

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
        public async Task<IActionResult> Archive(int id)
        {

            try
            {
                int companyId = User.Identity!.GetCompanyId();

                Project project = await _btProjectService.GetProjectAsync(id, companyId);

                await _btProjectService.ArchiveProjectAsync(project);

                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                throw;
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMemberToProject(int projectId, IEnumerable<string> selectedMembers)
        {
            int companyId = User.Identity!.GetCompanyId();

            if (selectedMembers != null)
            {

                //remove members

                await _btProjectService.RemoveMembersFromProjectAsync(projectId, companyId);

                //Add members

                await _btProjectService.AddMembersToProjectAsync(selectedMembers, projectId, companyId);

                return RedirectToAction("Details", new { id = projectId });
            }

            List<BTUser> submitters = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.Submitter), companyId);
            List<BTUser> developers = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.Developer), companyId);
            List<BTUser> userList = submitters.Concat(developers).ToList();
            IEnumerable<BTUser> projectManagers = await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), companyId);

            ViewData["ProjectManagersId"] = new SelectList(projectManagers, "Id", "FullName");
            ViewData["MembersId"] = new MultiSelectList(userList, "Id", "FullName");
            return RedirectToAction("Detail", new { id = projectId});

        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddMemberToProject() 
        //{

        //    return View();
        //}

    }
}
