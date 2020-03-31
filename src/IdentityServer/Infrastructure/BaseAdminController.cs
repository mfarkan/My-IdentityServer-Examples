using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Infrastructure
{
    [Authorize(Roles = "CourierManageGodMode")]
    public class BaseAdminController : Controller
    {
    }
}
