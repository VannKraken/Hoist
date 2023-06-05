using Hoist.Models;

namespace Hoist.Services.Interfaces
{
    public interface IBTProjectService
    {

        #region Project CRUD

        public Task AddProjectAsync(Project project);

        public Task<Project> GetProjectAsync(int? projectId, int? companyId);
        public Task<IEnumerable<Project>> GetProjectsAsync(int companyId);
        public Task<IEnumerable<Project>> GetArchivedProjectsAsync(int companyId);
        public Task<BTUser> GetUserProjectsAsync(int companyId, string? userId);
        public Task<IEnumerable<Project>> GetUserProjectsListAsync(int? companyId, string? userId);


        public Task UpdateProjectAsync(Project project);
        public Task SaveProjectAsync();
        public Task ArchiveProjectAsync(Project project);
        public Task RestoreProjectAsync(int projectId, int companyId);



        #endregion

        #region Members
        public Task<bool> AddMemberToProjectAsync(BTUser? member, int? projectId);
        public Task AddMembersToProjectAsync(IEnumerable<string> userIds, int? projectId, int? companyId);
        public Task RemoveMembersFromProjectAsync(int? projectId, int? companyId);

        public Task<bool> RemoveMemberFromProjectAsync(BTUser? member, int? projectId);
        #endregion

        #region Project Manager


        public Task<bool> AddProjectManagerAsync(string? userId, int? projectId);

        public Task<BTUser> GetProjectManagerAsync(int? projectId);

        public Task RemoveProjectManagerAsync(int? projectId);

        
        #endregion

        #region Project Extended Methods

        public Task<IEnumerable<ProjectPriority>> GetProjectPrioritiesAsync();

        public bool ProjectExists(int projectId);

        #endregion

    }
}
