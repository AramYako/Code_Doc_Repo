using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace API_Azure.Controllers
{
    //This also works
    //Authorize(Roles = "Management")]    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : Controller
    {

        [HttpGet]
        [Route("GetRoles")]
        [Authorize(Roles = "RoleManagementt")]
        public List<int> GetValuesRoles()
        {
            return new List<int>() { 1, 2, 5 };
        }


        //Require: Member in all roles 
        [HttpGet]
        [Route("Admin")]
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Adminn")]
        public List<int> Admin()
        {
            return new List<int>() { 1, 2, 5 };
        }

        //Require: Member in one of the role 
        [HttpGet]
        [Route("Adminy")]
        [Authorize(Roles = "Admin,Adminn")]
        public List<int> Adminy()
        {
            return new List<int>() { 1, 2, 5 };
        }
    }
}
