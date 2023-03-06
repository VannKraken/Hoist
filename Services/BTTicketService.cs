using Hoist.Models;
using Hoist.Services.Interfaces;

namespace Hoist.Services
{
    public class BTTicketService : IBTTicketService
    {
        public Task AddTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        public Task ArchiveTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        public Task<Ticket> GetTicketAsync(int ticketId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }
    }
}
