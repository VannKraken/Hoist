using Hoist.Data;
using Hoist.Models;
using Hoist.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using System.ComponentModel.Design;

namespace Hoist.Services
{
    public class BTTicketService : IBTTicketService
    {

        private readonly ApplicationDbContext _context;

        public BTTicketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddTicketAsync(Ticket ticket)
        {
            _context.Add(ticket);
            await _context.SaveChangesAsync();
        }

        //public async Task<TicketStatus> GetTicketStatusByName(string statusName)
            

        public async Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment)
        {
            try
            {
                await _context.AddAsync(ticketAttachment);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task ArchiveTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Ticket>> GetProjectTicketsAsync(int? projectId, int? companyid)
        {
            throw new NotImplementedException();
        }

        public async Task<Ticket> GetTicketAsync(int ticketId)
        {
            Ticket? ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

            return ticket;
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByPriority(int? ticketPriorityId, int? companyId)
        {
            IEnumerable<Ticket> tickets = await _context.Tickets.Where(t => t.TicketPriorityId == ticketPriorityId)
                                                                  .Include(t => t.Project)
                                                                    .ThenInclude(p => p.CompanyId == companyId ).ToListAsync();

            return tickets;

        }

        public async Task<IEnumerable<Ticket>> GetTicketsByStatus(int? ticketStatusId, int? companyId)
        {
            IEnumerable<Ticket> tickets = await _context.Tickets.Where(t => t.TicketStatusId == ticketStatusId)
                                                                  .Include(t => t.Project)
                                                                    .ThenInclude(p => p.CompanyId == companyId).ToListAsync();
            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByType(int? ticketTypeId, int? companyId)
        {
            IEnumerable<Ticket> tickets = await _context.Tickets.Where(t => t.TicketTypeId == ticketTypeId)
                                                                  .Include(t => t.Project)
                                                                    .ThenInclude(p => p.CompanyId == companyId).ToListAsync();
            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetUserTicketsAsync(string? userId, int? companyId)
        {
            IEnumerable<Ticket> tickets = await _context.Tickets.Where(t => t.SubmitterUserId == userId || t.DeveloperUserId == userId)
                                                                    .Include(t => t.Project)
                                                                    .ToListAsync();
            return tickets;
        }

        public Task UpdateTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }
    }
}
