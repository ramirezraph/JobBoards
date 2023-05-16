using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace JobBoards.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ApiController : ControllerBase
{

}