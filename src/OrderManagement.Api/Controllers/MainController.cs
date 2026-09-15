using Microsoft.AspNetCore.Mvc;

namespace OrderManagement.Api.Controllers;

[ApiController]
public abstract class MainController : ControllerBase
{
    protected ActionResult NotFoundResponse() => NotFound();
}
