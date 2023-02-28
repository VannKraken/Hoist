using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hoist.Models
{
    public class Invite
    {
        public int Id { get; set; }

        public DateTime InviteDate { get; set; }

        public DateTime? JoinDate { get; set; }

        public Guid CompanyToken { get; set; }

        //FK
        public int CompanyId { get; set; }

        public int ProjectId { get; set; }

        [Required]
        public string? InvitorId { get; set; }

        public string? InviteeId { get; set; }

        //

        [Required]
        [Display(Name = "Invitee Email")]
        public string? InviteeEmail { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(40, ErrorMessage = " The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string? InviteeFirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(40, ErrorMessage = " The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string? InviteeLastName { get; set; }

        [NotMapped]
        public string? InviteeFullName { get { return $"{InviteeFirstName} {InviteeLastName}"; } }

     

        public string? Message { get; set; }

        public bool IsValid { get; set; }

        //Navigation
         public virtual Company? Company { get; set; }

        public virtual Project? Project { get; set; }

        public virtual HoistUser? Invitor { get; set; }
        public virtual HoistUser? Invitee { get; set; }
    }
}
