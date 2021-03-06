using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace API_Azure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        static readonly string[] scopeRequiredByApi = new string[] { "Management" };
        const string managementScope = "Management";
        const string fileReadScope = "Files.Read";

        [HttpGet]
        [Route("GetValueScope")]
        public async Task<int> GetValueScope()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            return 9;
        }

        [HttpGet]
        [Route("GetValueRole")]
        public async Task<int> GetValueRole()
        {
            HttpContext.ValidateAppRole(scopeRequiredByApi);

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            return 9;
        }

        [HttpGet]
        [Route("GetValuesScopes")]
        [RequiredScope(managementScope)]
        [RequiredScope(fileReadScope)]
        public List<int> GetValuesScopes()
        {
            return new List<int>() { 1, 2, 5 };
        }

        [HttpGet]
        [Route("GetValuess")]
        public List<int> GetValuess()
        {
            return new List<int>() { 1, 2, 5 };
        }
    }
}