using System.ComponentModel.DataAnnotations;

namespace Hoist.Models
{
    public class NotificationType
    {
        public int Id { get; set; }

        [Display(Name ="Type")]
        public string? Name { get; set; }
    }
}
