using Hoist.Data;
using Hoist.Models;
using Hoist.Models.Enums;
using Hoist.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace Hoist.Services
{


    public class BTNotifications : IBTNotifications
    {
        private readonly IEmailSender _emailService;
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _btRolesService;

        public BTNotifications(IEmailSender emailService, ApplicationDbContext context, IBTRolesService btRolesService)
        {
            _emailService = emailService;
            _context = context;
            _btRolesService = btRolesService;
        }

        public async Task AddNotificationAsync(Notification? notification)
        {
            try
            {

                if (notification != null)
                {
                    await _context.AddAsync(notification);
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task AdminNotificationAsync(Notification? notification, int? companyId)
        {
            try
            {
                if (notification != null && companyId != null)
                {
                    IEnumerable<string> adminIds = (await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.Admin), companyId.Value)).Select(u => u.Id);

                    foreach (string adminId in adminIds)
                    {
                        notification.Id = 0;
                        notification.RecipientId = adminId;
                        await _context.AddAsync(notification);
                    }

                    await _context.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Notification>> GetNotificationByUserId(string? userId)
        {
            try
            {
                List<Notification> notifications = new();
                    if (!string.IsNullOrEmpty(userId))
                    {

                        notifications = await _context.Notifications.Where(n => n.SenderId == userId || n.RecipientId == userId)
                                                                    .Include(n => n.Recipient)
                                                                    .Include(n => n.Sender)
                                                                    .ToListAsync();

                    }
                return notifications; 
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> SendAdminEmailNotificationAsync(Notification? notification, string? emailSubject, int? companyId)
        {
            try
            {
                if (notification != null)
                {
                    IEnumerable<string> adminEmails = (await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.Admin), companyId.Value))!.Select(u => u.Email)!;

                    foreach (string email in adminEmails)
                    {                     
                        await _emailService.SendEmailAsync(email, emailSubject!, notification.Message!);
                    }
                    return true;
                }

                return false;

                
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> SendEmailNotificationAsync(Notification? notification, string? emailSubject)
        {
            try
            {
                if (notification != null)
                {
                    BTUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == notification.RecipientId);
                    string? userEmail = user.Email;

                    if (userEmail != null) 
                    {
                        await _emailService.SendEmailAsync(userEmail, emailSubject!, notification.Message!);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
