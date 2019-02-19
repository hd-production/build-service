using Microsoft.AspNetCore.Mvc;

namespace HdProduction.ContentStorage.Controllers
{
  [Route("")]
  [ApiController]
  public class ValuesController : ControllerBase
  {
    [HttpGet]
    public string Index()
    {
      return "HdProduction.ContentStorage is running";
    }
  }
}