﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hoist.Models.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public BTUser? BTUser { get; set; }

        public MultiSelectList? Roles { get; set; }

        public List<string>? SelectedRoles { get; set; }
    }
}
