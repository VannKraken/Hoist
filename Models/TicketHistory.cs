using System.ComponentModel.DataAnnotations;

namespace Hoist.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }

        [Required]
        public string? HoistUserId { get; set;}

        public int TicketId { get; set; }


        public string? PropertyName{get; set;}

        public string? Description { get; set;}

        public DateTime Created { get; set;}

        public string? OldValue { get; set;}

        public string? NewValue { get; set;}

        //Navigation

        public virtual Ticket? Ticket { get; set;}

        public virtual HoistUser? HoistUser { get; set;}



    }
}
