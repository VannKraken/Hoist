﻿using Hoist.Models;

namespace Hoist.Services.Interfaces
{
    public interface IBTNotifications
    {


        public Task AddNotificationAsync(Notification? notification);
       
        public Task AdminNotificationAsync(Notification? notification, int? companyId);
        public Task<List<Notification>> GetNotificationByUserId(string? userId);
        public Task<bool> SendAdminEmailNotificationAsync(Notification? notification, string? emailSubject, int? companyId);
        public Task<bool> SendEmailNotificationAsync(Notification? notification, string? emailSubject);

        public Task<bool> MarkViewed(int? notificationId);
    }
}
