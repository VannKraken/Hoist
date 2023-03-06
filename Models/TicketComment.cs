using System.ComponentModel.DataAnnotations;

namespace Hoist.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public string? BTUserId { get; set; }

        [Required]
        [StringLength(10000, ErrorMessage = " The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string? Comment{ get; set; }


        [DataType(DataType.Date)]
        public DateTime Created { get; set; }


        //Navigation

        public virtual Ticket? Ticket { get; set; }

        public virtual BTUser? BTUser { get; set; }

    }
}
