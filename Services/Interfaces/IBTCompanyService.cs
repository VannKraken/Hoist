﻿using Hoist.Models;

namespace Hoist.Services.Interfaces
{
    public interface IBTCompanyService
    {

        public Task<Company> GetEverythingForCompanyAsync(int? companyId);

        public Task<BTUser> GetMemberAsync(string? userId, int? companyId);

        public Task<List<BTUser>> GetMembersAsync(int? companyId);

        public  Task<List<Notification>> GetMemberNotifications(string? userId, int? companyId);





    }
}
