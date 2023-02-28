using System.ComponentModel.DataAnnotations;

namespace Hoist.Models
{
    public class Notification
    {

        public int Id { get; set; }

        //Fk
        public int ProjectId { get; set; }

        public int TicketId { get; set; }
        [Required]
        public string? SenderId { get; set; }
        [Required]
        public string? RecipientId { get; set; }

        public int NotificationTypeId { get; set; }
        //

        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Message { get; set; }

        public DateTime Created { get; set; }

        public bool HasBeenViewed { get; set; }

        //Navigation Properties

        public virtual NotificationType? NotifcationType { get; set; }

        public virtual Ticket? Ticket { get; set; }

        public virtual Project? Project { get; set; }

        public virtual HoistUser? Sender { get; set; }

        public virtual HoistUser? Recipient { get; set; }







    }
}
