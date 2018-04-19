using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace SpecSelRepos.Models.AccountViewModels
{
    public class ManageRolesViewModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ManageRolesViewModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            Users = new SelectList(userManager.Users.ToList());
            Roles = new SelectList(roleManager.Roles.ToList());
        }

        public SelectList Users;
        public SelectList Roles;
        public string ManageRolesUser { get; set; }//contains the specific user the user selects

        public string ManageRolesRole { get; set; }//contains the specific role the user selects

        public List<ApplicationUser> GetUsersInRole(string role)
        {
            return _userManager.GetUsersInRoleAsync(role).Result.ToList();
        }
    }
}
