using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hoist.Models
{
    public class Company
    {
    
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        public byte[]? ImageData { get; set; }

        public string? ImageType { get; set; }


        [NotMapped]
        public virtual IFormFile? ImageFile { get; set; }

        //Navigation

        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();
        public virtual ICollection<HoistUser> Members { get; set; } = new HashSet<HoistUser>();
        public virtual ICollection<Invite> Invites { get; set; } = new HashSet<Invite>();


    }
}
