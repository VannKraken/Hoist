﻿using System;
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

namespace Hoist.Controllers
{
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

        // GET: Companies
        public async Task<IActionResult> Index()
        {
              return _context.Companies != null ? 
                          View(await _context.Companies.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Companies'  is null.");
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ImageFileData,ImageFileType")] Company company)
        {
            if (ModelState.IsValid)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ImageFileData,ImageFileType")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
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
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Companies == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Companies'  is null.");
            }
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
          return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }



        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {

            int? companyId = User.Identity!.GetCompanyId();

            IEnumerable<BTUser> members = await _btCompanyService.GetMembersAsync(companyId);
            List<ManageUserRolesViewModel> userModels = new List<ManageUserRolesViewModel>(); 

            foreach (BTUser member in members)
            {
                IEnumerable<string> currentRoles = await _btRolesService.GetUserRolesAsync(member);

                ManageUserRolesViewModel user = new()
                {
                    

                    BTUser = member,
                    Roles = new MultiSelectList(await _btRolesService.GetRolesAsync(), "Name", "Name",currentRoles, "Name")

                    
                };

                userModels.Add(user);
            }

            return View(userModels);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel viewModel)
        {
            int companyId = User.Identity!.GetCompanyId();

            // 2 Instantiate the BTUser
            BTUser? btUser = await _context.Users.FindAsync(viewModel.BTUser.Id);

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


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ManageUserRoles([Bind("BTUser, Roles, SelectedRoles")] ManageUserRolesViewModel viewModel)
        ////
        //    int companyId = User.Identity!.GetCompanyId();

        //    BTUser? user = await _context.Users.FindAsync(viewModel.BTUser.Id);

        //    await _btRolesService.GetUserRolesAsync(user);

        //    if (viewModel.SelectedRoles != null)
        //    {
        //        await _btRolesService.RemoveUserFromRolesAsync(user, viewModel.SelectedRoles);

        //        foreach (string selectRole in viewModel.SelectedRoles)
        //        {
        //            await _btRolesService.AddUserToRoleAsync(viewModel.BTUser, selectRole);
        //        }
        //    }




        //}


    }
}
