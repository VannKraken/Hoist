using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hoist.Models.ViewModels
{
    public class AssignPMViewModel
    {
        public Project? Project { get; set; }

        public SelectList? ProjectManagerList { get; set; }

        public string? ProjectManagerId { get; set; }
    }
}
