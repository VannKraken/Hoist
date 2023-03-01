using System.ComponentModel.DataAnnotations;

namespace Hoist.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }

        [Required]
        public string? BTUserId { get; set;}

        public int TicketId { get; set; }

        [Display(Name = "Updated Property")]      
        public string? PropertyName{get; set;}

        [StringLength(5000, ErrorMessage = " The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string? Description { get; set;}


        [DataType(DataType.DateTime)]
        public DateTime Created { get; set;}

        public string? OldValue { get; set;}

        public string? NewValue { get; set;}

        //Navigation

        public virtual Ticket? Ticket { get; set;}

        public virtual BTUser? BTUser { get; set;}



    }
}
