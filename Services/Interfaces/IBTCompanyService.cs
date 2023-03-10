using Hoist.Models;

namespace Hoist.Services.Interfaces
{
    public interface IBTCompanyService
    {

        public Task<Company> GetCompanyInfoAsync(int? companyId);

        public Task<BTUser> GetMemberAsync(string? userId, int? companyId);

        public Task<List<BTUser>> GetMembersAsync(int? companyId);

        

    }
}
