using Hoist.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hoist.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        [Required]
        public string? BTUserId { get; set; }


        public string? Description { get; set; }


        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        public byte[]? FileData { get; set; }


        public string? FileName { get; set; }

        public string? FileType { get; set; }


        [NotMapped]
        [Display(Name ="Select a File")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1024*1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".pdf" } )]
        public virtual IFormFile? FormFile { get; set; }

        //Navigation

        public virtual Ticket? Ticket { get; set; }

        public virtual BTUser? BTUser { get; set; }


    }
}
