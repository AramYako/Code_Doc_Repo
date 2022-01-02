using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace API_Azure.Controllers
{
    [Authorize]
    public class ScopeController : ControllerBase
    {
        static readonly string[] scopeRequiredByApi = new string[] { "Management" };
        const string managementScope = "Management";
        const string fileReadScope = "Files.Read";

        [HttpGet]
        [Route("GetScope")]
        public async Task<int> GetScope()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            return 9;
        }


        [HttpGet]
        [Route("GetScopes")]
        [RequiredScope(managementScope)]
        [RequiredScope(fileReadScope)]
        public List<int> GetScopes()
        {
            return new List<int>() { 1, 2, 5 };
        }
    }
}
