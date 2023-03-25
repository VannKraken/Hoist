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
using Hoist.Models.ViewModels;
using Hoist.Extensions;
using Microsoft.AspNetCore.Identity;
using Hoist.Services;
using Hoist.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;

namespace Hoist.Controllers
{

    [Authorize]
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTRolesService _btRolesService;
        private readonly IBTCompanyService _btCompanyService;



        public CompaniesController(ApplicationDbContext context, IBTRolesService btRolesService, IBTCompanyService btCompanyService, UserManager<BTUser> userManager)
        {
            _context = context;
            _btRolesService = btRolesService;
            _btCompanyService = btCompanyService;
            _userManager = userManager;
        }


        //#region Companies CRUD
        //// GET: Companies
        //public async Task<IActionResult> Index()
        //{
        //    return _context.Companies != null ?
        //                View(await _context.Companies.ToListAsync()) :
        //                Problem("Entity set 'ApplicationDbContext.Companies'  is null.");
        //}

        //// GET: Companies/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.Companies == null)
        //    {
        //        return NotFound();
        //    }

        //    var company = await _context.Companies
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (company == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(company);
        //}

        //// GET: Companies/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Companies/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name,Description,ImageFileData,ImageFileType")] Company company)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(company);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(company);
        //}

        //// GET: Companies/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Companies == null)
        //    {
        //        return NotFound();
        //    }

        //    var company = await _context.Companies.FindAsync(id);
        //    if (company == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(company);
        //}

        //// POST: Companies/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ImageFileData,ImageFileType")] Company company)
        //{
        //    if (id != company.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(company);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CompanyExists(company.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(company);
        //}

        //// GET: Companies/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Companies == null)
        //    {
        //        return NotFound();
        //    }

        //    var company = await _context.Companies
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (company == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(company);
        //}

        //// POST: Companies/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Companies == null)
        //    {
        //        return Problem("Entity set 'ApplicationDbContext.Companies'  is null.");
        //    }
        //    var company = await _context.Companies.FindAsync(id);
        //    if (company != null)
        //    {
        //        _context.Companies.Remove(company);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        //#endregion


        private bool CompanyExists(int id)
        {
            return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        [Authorize]
        public async Task<IActionResult> Info()
        {
            int companyId = User.Identity.GetCompanyId();

            Company company = await _btCompanyService.GetCompanyInfo(companyId);

            return View(company);
        }

        [Authorize]
        public async Task<IActionResult> Members(int? pageNum)
        {
            int pageSize = 20;  //Number per page
            int page = pageNum ?? 1;  //Which page number clicked upon on the page.

            int companyId = User.Identity!.GetCompanyId();
            string? userId = _userManager.GetUserId(User);



            IPagedList<BTUser> members = (await _btCompanyService.GetMembersAsync(companyId)).ToPagedList(page, pageSize);


            return View(members);
        }




        #region Manage User Roles
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {

            int? companyId = User.Identity!.GetCompanyId();

            IEnumerable<BTUser> members = await _btCompanyService.GetMembersAsync(companyId);
            List<ManageUserRolesViewModel> userModels = new List<ManageUserRolesViewModel>();
            //MultiSelectList allRoles = new MultiSelectList(await _btRolesService.GetRolesAsync());

            List<IdentityRole> roles = (await _btRolesService.GetRolesAsync());
            IdentityRole? roleToRemove = roles.FirstOrDefault(r => r.Name == "DemoUser");


            roles.Remove(roleToRemove);            

            foreach (BTUser member in members)
            {
                IEnumerable<string> currentRoles = await _btRolesService.GetUserRolesAsync(member);

                ManageUserRolesViewModel user = new()
                {


                    BTUser = member,
                    Roles = new MultiSelectList( roles, "Name", "Name", currentRoles)


                };

                userModels.Add(user);
            }

            return View(userModels);

        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel viewModel)
        {
            int companyId = User.Identity!.GetCompanyId();

            // 2 Instantiate the BTUser
            BTUser? btUser = await _btCompanyService.GetMemberAsync(viewModel.BTUser!.Id, companyId);

            // 3 Get Roles for the User
            IEnumerable<string> currentRoles = await _btRolesService.GetUserRolesAsync(btUser);

            // 4 Get Selected Role(s) for the User submitted from the form
            IEnumerable<string> selectedRoles = viewModel.SelectedRoles.ToList();

            // 5 Remove current role(s) and Add new role
            await _btRolesService.RemoveUserFromRolesAsync(btUser, currentRoles);

            foreach (string role in selectedRoles)
            {
                await _btRolesService.AddUserToRoleAsync(btUser, role);
            }

            await _context.SaveChangesAsync();

            // 6 Navigate
            return RedirectToAction("ManageUserRoles");




        }
        #endregion




    }
}
