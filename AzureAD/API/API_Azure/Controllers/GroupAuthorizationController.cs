using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Azure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //This force the policy to be set in startup.
    [Authorize (Policy = "GroupPolicy")]
    public class GroupAuthorizationController : ControllerBase
    {
        [HttpGet]
        [Route("Group")]
        public ActionResult<string> GetGroup()
        {
            var spIdUserGroup = HttpContext.User.Claims.FirstOrDefault(t => t.Type == "groups" && t.Value == "3db4cb25-2a0a-46a1-8189-bf11266b5f50");

            if (spIdUserGroup != null)
                return Ok("success");

            return Unauthorized("No access");
        }

        //Startup rule protect this controller
        [HttpGet]
        [Route("GroupStartUp")]
        public ActionResult<string> GetStartupGroup()
        {
            return Ok("success");
        }
    }
}
