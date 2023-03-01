using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hoist.Models
{
    public class Company
    {
    
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = " The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string? Name { get; set; }

        [StringLength(40000, ErrorMessage = " The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string? Description { get; set; }

        public byte[]? ImageFileData { get; set; }

        public string? ImageFileType { get; set; }


        [NotMapped]
        public virtual IFormFile? ImageFormFile { get; set; }

        //Navigation

        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();
        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Invite> Invites { get; set; } = new HashSet<Invite>();


    }
}
