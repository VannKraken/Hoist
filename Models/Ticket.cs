using System.ComponentModel.DataAnnotations;

namespace Hoist.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        //Foreign Keys

        public int ProjectId { get; set; }

        public int TicketTypeId { get; set; }

        public int TicketStatusId { get; set; }

        public int TicketPriorityId { get; set; }


        public string? DeveloperUserId { get; set; }
        [Required]
        public string? SubmitterUserId { get; set; }


        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        public bool Archived { get; set; }

        public bool ArchivedByProject { get; set; }


        //Navigation

        public virtual Project? Project { get; set; }

        public virtual TicketType? TicketType { get; set; }

        public virtual TicketStatus? TicketStatus { get; set; }

        public virtual TicketPriority? TicketPriority { get; set; }

        public virtual HoistUser? DeveloperUser {get; set; }

        public virtual HoistUser? SubmitterUser { get; set; }

        public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();

        public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();

        public virtual ICollection<TicketHistory> History { get; set; } = new HashSet<TicketHistory>();



    }
}
