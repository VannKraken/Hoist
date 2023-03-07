using Hoist.Data;
using Hoist.Models;
using Hoist.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Hoist.Services
{
    public class BTRolesService : IBTRolesService
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;

        public BTRolesService(ApplicationDbContext context,
                              UserManager<BTUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        #region Add Users
        public async Task<bool> AddUserToRoleAsync(BTUser user, string roleName)
        {
            try
            {
                bool result  = (await _userManager.AddToRoleAsync(user, roleName)).Succeeded;
                return result;
            }
            catch (Exception)
            {

                throw;
            }

            
        }
        #endregion


        #region Get Users and Roles
        public async Task<List<IdentityRole>> GetRolesAsync()
        {

            try
            {
                List<IdentityRole> result = new();

                result = await _context.Roles.ToListAsync();

                return result;
            }
            catch (Exception)
            {

                throw;
            }
           

        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(BTUser user)
        {
            try
            {
                IEnumerable<string> result  = await _userManager.GetRolesAsync(user);

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<BTUser>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            try
            {
                List<BTUser> result = new();
                List<BTUser> users = new();

                users = (await _userManager.GetUsersInRoleAsync(roleName)).ToList();

                result = users.Where( u => u.CompanyId == companyId).ToList();

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion


        #region Remove User from Roles
        public async Task<bool> RemoveUserFromRoleAsync(BTUser user, string roleName)
        {

            try
            {
                bool result = (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;

                return result;
            }
            catch (Exception)
            {

                throw;
            }

            
        }

        public async Task<bool> RemoveUserFromRolesAsync(BTUser user, IEnumerable<string> roleNames)
        {

            try
            {
                bool result = (await _userManager.RemoveFromRolesAsync(user, roleNames)).Succeeded;

                return result;
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        #endregion



        public async Task<bool> IsUserInRoleAsync(BTUser member, string roleName)
        {

            try
            {
                bool result = await _userManager.IsInRoleAsync(member, roleName);

                return result;
            }
            catch (Exception)
            {

                throw;
            }
            
        }


    }
}
