using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpecSelRepos.Models.AccountViewModels
{
    public class ManageRolesViewModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;

        public ManageRolesViewModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            //_roleManager = roleManager;
            //List<ApplicationUser> users = ;
            //List<IdentityRole> roles = ;
            Users = new SelectList(userManager.Users.ToList());
            Roles = new SelectList(roleManager.Roles.ToList());
            //GetAsyncUsersInRole("admin").Wait();
        }


        public IList<ApplicationUser> DbUsers { get; set; }
        //public List<string> DbRoles { get; set; }

        public SelectList Users;
        public SelectList Roles;
        public string ManageRolesUser { get; set; }//contains the specific user the user selects

        public string ManageRolesRole { get; set; }//contains the specific role the user selects

        public List<ApplicationUser> GetUsersInRole(string role)
        {
            var users = _userManager.GetUsersInRoleAsync(role).Result.ToList();
            return users;
        }



    }
}
