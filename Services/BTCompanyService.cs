﻿using Hoist.Data;
using Hoist.Models;
using Hoist.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hoist.Services
{
    public class BTCompanyService : IBTCompanyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTFileService _btFileService;

        public BTCompanyService(ApplicationDbContext context, IBTFileService btFileService)
        {
            _context = context;
            _btFileService = btFileService;
        }

        public async Task<Company> GetCompanyInfo(int? companyId)
        {
            try
            {
                Company? company = await _context.Companies.Include(c => c.Members)
                                                           .Include(c => c.Projects)
                                                           .FirstOrDefaultAsync(c => c.Id == companyId);

                return company;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Company> GetEverythingForCompanyAsync(int? companyId)
        {


            try
            {
                Company? company = await _context.Companies
                                                 .Include(c => c.Projects)
                                                    .ThenInclude(p => p.Tickets)
                                                        .ThenInclude(t => t.SubmitterUser)
                                                 .Include(c => c.Projects)
                                                    .ThenInclude(p => p.Tickets)
                                                        .ThenInclude(t => t.DeveloperUser)
                                                 .Include(c => c.Projects)
                                                    .ThenInclude(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketPriority)
                                                 .Include(c => c.Projects)
                                                    .ThenInclude(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketStatus)
                                                 .Include(c => c.Projects)
                                                    .ThenInclude(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketType)
                                                 .Include(c => c.Projects)
                                                    .ThenInclude(p => p.Members)
                                                 .Include(c => c.Members)
                                                    .ThenInclude(m => m.Projects)
                                                 .Include(c => c.Invites)
                                                 .FirstOrDefaultAsync(c => c.Id == companyId);

                return company;
            }
            catch (Exception)
            {

                throw;
            }

            
                                                 
        }

        public async Task<IEnumerable<BTUser>> GetMembersAsync(int? companyId)
        {


            IEnumerable<BTUser> members = await _context.Users.Where(u => u.CompanyId == companyId)
                                                               .Include( u => u.Projects)
                                                               .ThenInclude(p => p.Tickets)
                                                               .ToListAsync();

            return members;

        }

        public async Task<List<Notification>> GetMemberNotifications(string? userId, int? companyId) 
        {
            List<Notification> notifications = await _context.Notifications.Where(n => n.SenderId == userId || n.RecipientId == userId)
                                                                     .Include(n => n.NotificationType)
                                                                     .Include(n => n.Sender)
                                                                     .Include(n => n.Recipient)
                                                                     .ToListAsync();

            return notifications;
        }

        public async Task<BTUser> GetMemberAsync(string? userId, int? companyId)
        {
            BTUser? member = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.CompanyId == companyId);

            return member;
        }

        public async Task UpdateMemberAsync(IFormFile imageFile, BTUser member)
        {
            if (imageFile != null)
            {
                member.ImageFileData = await _btFileService.ConvertFileToByteArrayAsync(imageFile);
                member.ImageFileType = member.ImageFormFile?.ContentType;
            }

            _context.Users.Update(member);
            await _context.SaveChangesAsync();
        }
    }
}
