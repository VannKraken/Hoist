using System.ComponentModel.DataAnnotations;

namespace Hoist.Models
{
    public class ProjectPriority
    {
        public int Id { get; set; }

        [Display(Name ="Priority")]
        public string? Name { get; set; }
    }
}
