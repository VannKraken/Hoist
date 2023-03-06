using Hoist.Models;

namespace Hoist.Services.Interfaces
{
    public interface IBTProjectService
    {

        #region Project CRUD

        public Task AddProjectAsync(Project project);

        public Task<Project> GetProjectAsync(int projectId, int companyId);
        public Task<IEnumerable<Project>> GetProjectsAsync(int companyId);
        public Task UpdateProjectAsync(Project project);
        public Task ArchiveProjectAsync(Project project);



        #endregion



        #region Project Extended Methods

        public Task<IEnumerable<ProjectPriority>> GetProjectPrioritiesAsync();

        public bool ProjectExists(int projectId);

        #endregion

    }
}
