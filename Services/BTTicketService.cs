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

        public async Task AddTicketComment(TicketComment ticketComment)
        {
            _context.Add(ticketComment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ArchiveTicketAsync(Ticket ticket)
        {

            try
            {
                ticket.Archived = true;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {

                throw;
            }
            

        }

        public async Task<IEnumerable<Ticket>> GetCompanyTicketsAsync(int? companyId)
        {
            IEnumerable<Ticket> tickets = await _context.Tickets.Include(t => t.DeveloperUser)
                                                                .Include(t => t.Project)
                                                                    .ThenInclude(p => p.Company)
                                                                    .Where(t => t.Project!.CompanyId == companyId)
                                                                .Include(t => t.SubmitterUser)
                                                                .Include(t => t.TicketPriority)
                                                                .Include(t => t.TicketStatus)
                                                                .Include(t => t.TicketType)
                                                                .Include(t => t.Attachments).ToListAsync();

            return tickets;
        }

        public Task<IEnumerable<Ticket>> GetProjectTicketsAsync(int? projectId, int? companyid)
        {
            throw new NotImplementedException();
        }

        public async Task<Ticket> GetTicketAsync(int? ticketId)
        {
            Ticket? ticket = await _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.Project)
                .Include(t => t.SubmitterUser)
                .Include(t => t.Comments)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .FirstOrDefaultAsync(m => m.Id == ticketId); ;

            return ticket;
        }

        public async Task<IEnumerable<TicketPriority>> GetTicketPriorities()
        {
            IEnumerable<TicketPriority> ticketPriorities = await _context.TicketPriorities.ToListAsync();
            return ticketPriorities;
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

        public async Task<IEnumerable<TicketStatus>> GetTicketStatuses()
        {
            IEnumerable<TicketStatus> ticketStatuses = await _context.TicketStatuses.ToListAsync();

            return ticketStatuses;
        }

        public async Task<IEnumerable<TicketType>> GetTicketTypes()
        {
            IEnumerable<TicketType> ticketTypes = await _context.TicketTypes.ToListAsync();

            return ticketTypes;
        }

        public async Task<IEnumerable<Ticket>> GetUserTicketsAsync(string? userId, int? companyId)
        {
            IEnumerable<Ticket> tickets = await _context.Tickets.Where(t => t.SubmitterUserId == userId || t.DeveloperUserId == userId)
                                                                    .Include(t => t.Project)
                                                                    .ToListAsync();
            return tickets;
        }

        public async Task<bool> TicketExists(int? ticketId)
        {
            return ( _context.Tickets?.Any(e => e.Id == ticketId)).GetValueOrDefault();
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            _context.Update(ticket);
            await _context.SaveChangesAsync();
        }
    }
}
