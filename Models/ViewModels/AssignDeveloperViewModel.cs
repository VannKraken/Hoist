using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hoist.Models.ViewModels
{
    public class AssignDeveloperViewModel
    {

        //Ticket Model

        public Ticket? Ticket { get; set; }
        //Developer List
        public SelectList? DeveloperList { get; set; }

        public string? DeveloperId { get; set; }

        

        //Selected Developer

    }
}
