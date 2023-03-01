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

        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }


        [Display(Name="Start Date")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }


        [Display(Name="End Date")]
        [DataType(DataType.DateTime)]
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
        
        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();

        public virtual ICollection<Ticket> Tickets { get; } = new HashSet<Ticket>();


    }
}
