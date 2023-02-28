using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hoist.Models
{
    public class Project
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public int ProjectPriorityId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }
        
        public DateTime Created { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool Archived { get; set; }

        //File Properties

        public byte[]? FileData { get; set; }

        public string? FileType { get; set; }


        [NotMapped]
        public virtual IFormFile? FormFile { get; set; }

        //Navigation

        public virtual Company? Company { get; set; }

        public virtual ProjectPriority? ProjectPriority { get; set; }
        
        public virtual ICollection<HoistUser> Members { get; set; } = new HashSet<HoistUser>();

        public virtual ICollection<Ticket> Tickets { get; } = new HashSet<Ticket>();


    }
}
