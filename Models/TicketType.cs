using System.ComponentModel.DataAnnotations;

namespace Hoist.Models
{
    public class TicketType
    {
        public int Id { get; set; }


        [Required]
        [Display(Name="Type")]
        public string? Name { get; set; }
    }
}
