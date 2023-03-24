using Hoist.Models;

namespace Hoist.Services.Interfaces
{
    public interface IBTTicketService
    {
        #region Ticket CRUD
        public Task AddTicketAsync(Ticket ticket);

        public Task<Ticket> GetTicketAsync(int? ticketId, int? companyId);
        public Task<Ticket> GetTicketSnapshotAsync(int? ticketId, int? companyId);

        public Task<IEnumerable<Ticket>> GetCompanyTicketsAsync(int? companyId);

        public Task<IEnumerable<Ticket>> GetProjectTicketsAsync(int? projectId, int? companyId);

        public Task<IEnumerable<Ticket>> GetUserTicketsAsync(string? userId, int? companyid);

        public Task<IEnumerable<Ticket>> GetTicketsByPriority(int? ticketPriorityId, int? companyId);

        public Task<IEnumerable<Ticket>> GetTicketsByStatus(int? ticketStatusId, int? companyId);

        public Task<IEnumerable<Ticket>> GetTicketsByType(int? ticketTypeId, int? companyId);

        public Task UpdateTicketAsync(Ticket ticket);

        public Task<bool> ArchiveTicketAsync(Ticket ticket);

        public Task<bool> TicketExists(int? ticketId);

        #endregion


        public Task AddTicketComment(TicketComment ticketComment);

        public Task<IEnumerable<TicketPriority>> GetTicketPriorities();
        public Task<IEnumerable<TicketStatus>> GetTicketStatuses();
        public Task<IEnumerable<TicketType>> GetTicketTypes();

        public Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment);

        public Task<TicketAttachment> GetTicketAttachmentByIdAsync(int ticketAttachmentId);
    }
}
