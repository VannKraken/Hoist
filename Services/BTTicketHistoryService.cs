using Hoist.Data;
using Hoist.Models;
using Hoist.Services.Interfaces;

namespace Hoist.Services
{
    public class BTTicketHistoryService : IBTTicketHistoryService
    {
        private readonly ApplicationDbContext _context;

        public BTTicketHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddHistoryAsync(Ticket? oldTicket, Ticket? newTicket, string? userId)
        {
            try
            {
                if (oldTicket == null && newTicket != null)
                {
                    TicketHistory ticketHistory = new()
                    {
                        TicketId= newTicket.Id,
                        PropertyName = string.Empty,
                        OldValue = string.Empty,
                        NewValue = string.Empty,
                        Created = DataUtility.GetPostGresDate(DateTime.Now),
                        BTUserId = userId,
                        Description = "Ticket Created."
                    };

                    try
                    {
                        await _context.TicketHistories.AddAsync(ticketHistory);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                else if (oldTicket != null && newTicket != null)
                {

                    //---------------------------Check Ticket Title-----------------------------------//

                    if (oldTicket.Title != newTicket.Title)
                    {
                        TicketHistory ticketHistory = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "Title",
                            OldValue = oldTicket.Title,
                            NewValue = newTicket.Title,
                            Created = DataUtility.GetPostGresDate(DateTime.Now),
                            BTUserId = userId,
                            Description = $"Title changed from {oldTicket.Title} to {newTicket.Title}."
                        };

                        await _context.TicketHistories.AddAsync(ticketHistory);
                    }

                    //---------------------------Check Ticket Description-----------------------------//

                    if (oldTicket.Description != newTicket.Description)
                    {
                        TicketHistory ticketHistory = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "Description",
                            OldValue = oldTicket.Description,
                            NewValue = newTicket.Description,
                            Created = DataUtility.GetPostGresDate(DateTime.Now),
                            BTUserId = userId,
                            Description = $"Description changed."
                        };

                        await _context.TicketHistories.AddAsync(ticketHistory);
                    }

                    //---------------------------Check Ticket Priority-----------------------------//

                    if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
                    {
                        TicketHistory ticketHistory = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "Priority",
                            OldValue = oldTicket.TicketPriority?.Name,
                            NewValue = newTicket.TicketPriority?.Name,
                            Created = DataUtility.GetPostGresDate(DateTime.Now),
                            BTUserId = userId,
                            Description = $"Priority changed from {oldTicket.TicketPriority?.Name} to {newTicket.TicketPriority?.Name}."
                        };

                        await _context.TicketHistories.AddAsync(ticketHistory);
                    }

                    //---------------------------Check Ticket Type---------------------------------//

                    if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
                    {
                        TicketHistory ticketHistory = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "Type",
                            OldValue = oldTicket.TicketType?.Name,
                            NewValue = newTicket.TicketType?.Name,
                            Created = DataUtility.GetPostGresDate(DateTime.Now),
                            BTUserId = userId,
                            Description = $"Type changed from {oldTicket.TicketType?.Name} to {newTicket.TicketType?.Name}."
                        };

                        await _context.TicketHistories.AddAsync(ticketHistory);
                    }

                    //---------------------------Check Ticket Status-------------------------------//

                    if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
                    {
                        TicketHistory ticketHistory = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "Status",
                            OldValue = oldTicket.TicketStatus?.Name,
                            NewValue = newTicket.TicketStatus?.Name,
                            Created = DataUtility.GetPostGresDate(DateTime.Now),
                            BTUserId = userId,
                            Description = $"Status changed from {oldTicket.TicketStatus?.Name} to {newTicket.TicketStatus?.Name}."
                        };

                        await _context.TicketHistories.AddAsync(ticketHistory);
                    }

                    //---------------------------Check Ticket Status-------------------------------//

                    if (oldTicket.DeveloperUserId != newTicket.DeveloperUserId)
                    {
                        TicketHistory ticketHistory = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "Developer",
                            OldValue = oldTicket.DeveloperUser?.FullName ?? "Not Assigned",
                            NewValue = newTicket.DeveloperUser?.FullName,
                            Created = DataUtility.GetPostGresDate(DateTime.Now),
                            BTUserId = userId,
                            Description = $"Developer changed from {oldTicket.DeveloperUser?.FullName} to {newTicket.DeveloperUser?.FullName}."
                        };

                        await _context.TicketHistories.AddAsync(ticketHistory);
                    }

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task AddHistoryAsync(int? ticketId, string? model, string? userId)
        {
            try
            {
                Ticket? ticket = await _context.Tickets.FindAsync(ticketId);

                string description = model!.ToLower().Replace("ticket", "");

                description = $"New {description} added to ticket: {ticket?.Title}.";

                if(ticket != null)
                {
                    TicketHistory ticketHistory = new()
                    {
                        TicketId = ticket.Id,
                        PropertyName = model,
                        OldValue = string.Empty,
                        NewValue = string.Empty,
                        Created = DataUtility.GetPostGresDate(DateTime.Now),
                        BTUserId = userId,
                        Description = description
                    };
                    await _context.TicketHistories.AddAsync(ticketHistory);
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<List<TicketHistory>> GetCompanyTicketHistoriesAsync(int? companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TicketHistory>> GetProjectTicketHistoriesAsync(int? projectId, int? companyId)
        {
            throw new NotImplementedException();
        }
    }
}
