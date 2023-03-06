using Hoist.Data;
using Hoist.Models;
using Hoist.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hoist.Services
{
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;


        public BTProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

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

        public async Task ArchiveProjectAsync(Project project)
        {
            try
            {
                if (project != null)
                {
                    project.Archived = true;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Project> GetProjectAsync(int projectId, int companyId)
        {

            try
            {
                Project? project = await _context.Projects
                                                            .Include(p => p.Company)
                                                            .Include(p => p.ProjectPriority)
                                                            .Include(p => p.Tickets)
                                                                .ThenInclude(t => t.DeveloperUser)
                                                            .Include(p => p.Tickets)
                                                                .ThenInclude(t => t.SubmitterUser)
                                                            .FirstOrDefaultAsync(m => m.Id == projectId && m.CompanyId == companyId);

                return project;
            }
            catch (Exception)
            {

                throw;
            }
           
        }

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

        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
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

        
    }
}
