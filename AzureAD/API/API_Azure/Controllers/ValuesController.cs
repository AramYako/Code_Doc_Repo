using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace API_Azure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        static readonly string[] scopeRequiredByApi = new string[] { "Management" };
        const string managementScope = "Management";
        const string fileReadScope = "Files.Read";

        [HttpGet]
        [Route("GetValue")]
        public async Task<int> GetValue()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            return 9;
        }

        [HttpGet]
        [Route("GetValues")]
        [RequiredScope(managementScope)]
        [RequiredScope(fileReadScope)]
        public List<int> GetValues()
        {
            return new List<int>() { 1, 2, 5 };
        }

        [HttpGet]
        public List<int> Values()
        {
            return new List<int>() { 99999999 };
        }
    }
}
