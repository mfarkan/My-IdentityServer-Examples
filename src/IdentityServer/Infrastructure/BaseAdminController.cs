using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Infrastructure
{
    [Authorize(Policy = "SuperAdmin")]
    public class BaseAdminController : Controller
    {
    }
}
