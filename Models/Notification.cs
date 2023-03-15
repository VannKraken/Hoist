using System.ComponentModel.DataAnnotations;

namespace Hoist.Models
{
    public class Notification
    {

        public int Id { get; set; }

        //Fk
        public int ProjectId { get; set; }

        public int? TicketId { get; set; }

        [Required]
        public string? SenderId { get; set; }

        [Required]
        public string? RecipientId { get; set; }

        public int NotificationTypeId { get; set; }
    

        [Required]
        [Display(Name="Subject")]
        [StringLength(200, ErrorMessage = " The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string? Title { get; set; }


        [Required]
        [StringLength(10000, ErrorMessage = " The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string? Message { get; set; }


        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        public bool HasBeenViewed { get; set; }

        //Navigation Properties

        public virtual NotificationType? NotificationType { get; set; }

        public virtual Ticket? Ticket { get; set; }

        public virtual Project? Project { get; set; }

        public virtual BTUser? Sender { get; set; }

        public virtual BTUser? Recipient { get; set; }







    }
}
