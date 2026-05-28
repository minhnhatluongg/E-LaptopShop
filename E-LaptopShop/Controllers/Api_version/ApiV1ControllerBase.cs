using Microsoft.AspNetCore.Mvc;

namespace E_LaptopShop.Controllers.Api_version;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class ApiV1ControllerBase : ControllerBase { }
