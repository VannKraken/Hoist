using System.ComponentModel.DataAnnotations;

namespace Hoist.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public string? BTUserId { get; set; }

        [Required]
        public string? Comment{ get; set; }


        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }


        //Navigation

        public virtual Ticket? Ticket { get; set; }

        public virtual BTUser? BTUser { get; set; }

    }
}
