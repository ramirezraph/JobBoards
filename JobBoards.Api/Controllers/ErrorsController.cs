using Microsoft.AspNetCore.Mvc;

namespace JobBoards.Api.Controllers;


[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorsController : ControllerBase
{
    [Route("/error")]
    public IActionResult Error()
    {
        return Problem();
    }
}