using System.ComponentModel.DataAnnotations;

namespace Hoist.Models
{
    public class TicketStatus
    {
        public int Id { get; set; }


        [Required]
        [Display(Name="Status")]
        public string? Name { get; set; }
    }
}
