﻿using Hoist.Models;

namespace Hoist.Services.Interfaces
{
    public interface IBTTicketService
    {
        #region Ticket CRUD
        public Task AddTicketAsync(Ticket ticket);

        public Task<Ticket> GetTicketAsync(int ticketId);

        public Task UpdateTicketAsync(Ticket ticket);

        public Task ArchiveTicketAsync(Ticket ticket);

        #endregion
    }
}
