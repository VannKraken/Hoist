using Hoist.Data;
using Hoist.Models;
using Hoist.Models.Enums;
using Hoist.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hoist.Services
{
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _btRoleService;

        public BTProjectService(ApplicationDbContext context, IBTRolesService btRoleService)
        {
            _context = context;
            _btRoleService = btRoleService;
        }


        #region Project CRUD

        public async Task AddProjectAsync(Project project)
        {

            try
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Project> GetProjectAsync(int? projectId, int? companyId)
        {

            try
            {
                Project? project = await _context.Projects.Include(p => p.Company)
                                                          .Include(p => p.ProjectPriority)
                                                          .Include(p => p.Members)
                                                          .Include(p => p.Tickets)
                                                            .ThenInclude(t => t.DeveloperUser)
                                                          .Include(p => p.Tickets)
                                                            .ThenInclude(t => t.SubmitterUser)
                                                          .Include(p => p.Tickets)
                                                            .ThenInclude(t => t.TicketPriority)
                                                          .Include(p => p.Tickets)
                                                            .ThenInclude(t => t.TicketStatus)
                                                          .Include(p => p.Tickets)
                                                            .ThenInclude(t => t.TicketType)                                                          
                                                          .FirstOrDefaultAsync(m => m.Id == projectId && m.CompanyId == companyId);

                return project;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<IEnumerable<Project>> GetProjectsAsync(int companyId)
        {
            try
            {
                IEnumerable<Project> projects = await _context.Projects
                                            .Where(p => p.CompanyId == companyId && p.Archived == false)
                                            .Include(p => p.Tickets)
                                            .Include(p => p.Company)
                                            .Include(p => p.ProjectPriority)
                                            .Include(p => p.Members).ToListAsync();

                return projects;
            }
            catch (Exception)
            {

                throw;
            }


        }
        public async Task<BTUser> GetUserProjectsAsync(int companyId, string? userId)
        {
            try
            {
                //IEnumerable<Project> projects = await _context.Projects
                //                            .Where(p => p.CompanyId == companyId && p.Archived == false)
                //                            .Include(p => p.Tickets)
                //                            .Include(p => p.Company)
                //                            .Include(p => p.ProjectPriority)
                //                            .Include(p => p.Members.Where( m => m.Id == userId)).ToListAsync();


                BTUser? user = await _context.Users.Include(u => u.Projects).FirstOrDefaultAsync(u => u.Id == userId && u.CompanyId == companyId);

                return user;
            }
            catch (Exception)
            {

                throw;
            }


        }
        public async Task<IEnumerable<Project>> GetUserProjectsListAsync(int? companyId, string? userId)
        {
            try
            {
                IEnumerable<Project> projects = await _context.Projects
                                            .Where(p => p.CompanyId == companyId && p.Archived == false)
                                            .Include(p => p.Tickets)
                                            .Include(p => p.Company)
                                            .Include(p => p.ProjectPriority)
                                            .Include(p => p.Members)
                                            .Where(p => p.Members.Any(m => m.Id == userId)).ToListAsync();


                //BTUser? user = await _context.Users.Include(u => u.Projects).FirstOrDefaultAsync(u => u.Id == userId && u.CompanyId == companyId);

                return projects;
            }
            catch (Exception)
            {

                throw;
            }


        }

        public async Task UpdateProjectAsync(Project project)
        {
            var trackedEntity = _context.Projects.Local.SingleOrDefault(p => p.Id == project.Id);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        
        }

        public async Task SaveProjectAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task ArchiveProjectAsync(Project project)
        {
            try
            {
                if (project != null)
                {
                    project.Archived = true;
                    _context.Update(project);

                    foreach (Ticket ticket in project.Tickets)
                    {
                        ticket.Archived = true;
                        ticket.ArchivedByProject = true;
                        _context.Update(ticket);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Project Members Methods

        public async Task<bool> AddMemberToProjectAsync(BTUser? member, int? projectId)
        {
            try
            {
                Project project = await GetProjectAsync(projectId.Value, member.CompanyId);

                bool onProject = project.Members.Contains(member);

                if (!onProject)
                {
                    project.Members.Add(member);
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task AddMembersToProjectAsync(IEnumerable<string> userIds, int? projectId, int? companyId)
        {
            try
            {

                Project? project = await GetProjectAsync(projectId, companyId);               

                foreach (string userId in userIds)
                {
                    BTUser? user = await _context.Users.FindAsync(userId);

                    if (project != null && user != null)
                    {
                        bool OnProject = project.Members.Any(m => m.Id == userId);

                        if (!OnProject)
                        {
                            project.Members.Add(user);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }   
                
                await _context.SaveChangesAsync();
                
                
                
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> RemoveMemberFromProjectAsync(BTUser? member, int? projectId)
        {
            try
            {
                Project project = await GetProjectAsync(projectId.Value, member.CompanyId);

                bool onProject = project.Members.Contains(member);

                if (onProject)
                {
                    project.Members.Remove(member);
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RemoveMembersFromProjectAsync(int? projectId, int? companyId)
        {
            try
            {
                Project? project = await GetProjectAsync(projectId, companyId);

                foreach (BTUser member in project.Members)
                {
                    if (!await _btRoleService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager)))
                    {
                        project.Members.Remove(member);
                    }
                    else
                    { 
                        continue; 
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Project Manager Methods
        public async Task<bool> AddProjectManagerAsync(string? userId, int? projectId)
        {

            try
            {
                BTUser? currentPM = await GetProjectManagerAsync(projectId);
                BTUser? selectedPM = await _context.Users.FindAsync(userId);

                //Remove current project manager
                if (currentPM != null)
                {
                    await RemoveProjectManagerAsync(projectId);
                }

                //Add new project manager.
                try
                {
                    await AddMemberToProjectAsync(selectedPM!, projectId);

                    return true;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<BTUser> GetProjectManagerAsync(int? projectId)
        {
            try
            {
                Project? project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);
                // Going through each member in the project to see if the user is in the role of Project Manager.
                foreach (BTUser member in project!.Members)
                {
                    if (await _btRoleService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager)))
                    {
                        return member;
                    }
                }

                return null!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RemoveProjectManagerAsync(int? projectId)
        {
            try
            {
                Project? project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);
                // Going through each member in the project to see if the user is in the role of Project Manager.
                foreach (BTUser member in project!.Members)
                {
                    if (await _btRoleService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager)))
                    {
                        await RemoveMemberFromProjectAsync(member, projectId);
                    }
                }

                
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion


        #region Extended Project Methods

        public async Task<IEnumerable<ProjectPriority>> GetProjectPrioritiesAsync()
        {
            try
            {
                IEnumerable<ProjectPriority> priorities = await _context.ProjectPriorities.ToListAsync();

                return priorities;
            }
            catch (Exception)
            {

                throw;
            }


        }





        public bool ProjectExists(int projectId)
        {
            try
            {
                return (_context.Projects?.Any(e => e.Id == projectId)).GetValueOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
        }

        
        #endregion













    }
}
