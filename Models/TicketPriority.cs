using System.ComponentModel.DataAnnotations;

namespace Hoist.Models
{
    public class TicketPriority
    {
        public int Id { get; set; }


        [Required]
        [Display(Name="Priority")]
        public string? Name { get; set; }
    }
}
