using Hoist.Models;

namespace Hoist.Services.Interfaces
{
    public interface IBTTicketService
    {
        #region Ticket CRUD
        public Task AddTicketAsync(Ticket ticket);

        public Task<Ticket> GetTicketAsync(int ticketId);

        public Task<IEnumerable<Ticket>> GetProjectTicketsAsync(int? projectId, int? companyid);

        public Task<IEnumerable<Ticket>> GetUserTicketsAsync(string? userId, int? companyid);

        public Task<IEnumerable<Ticket>> GetTicketsByPriority(int? ticketPriorityId, int? companyId);

        public Task<IEnumerable<Ticket>> GetTicketsByStatus(int? ticketStatusId, int? companyId);

        public Task<IEnumerable<Ticket>> GetTicketsByType(int? ticketTypeId, int? companyId);

        public Task UpdateTicketAsync(Ticket ticket);

        public Task ArchiveTicketAsync(Ticket ticket);

        #endregion

        public Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment);
    }
}
