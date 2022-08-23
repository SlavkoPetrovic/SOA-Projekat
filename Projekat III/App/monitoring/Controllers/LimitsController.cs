using Microsoft.AspNetCore.Mvc;
using monitoring.Limit;

namespace monitoring.Controllers;

[ApiController]
[Route("[controller]")]
public class LimitsController : ControllerBase
{
    private readonly Limits limits;
    public LimitsController(Limits limits)
    {
        this.limits = limits;
    }
    [Route("ph/{min}/{max}")]
    [HttpPost]
    public ActionResult ph(float min, float max)
    {
        limits.valueLimits["ph"] = new float[] { min, max };
        return Ok();
    }
    [Route("Organic_carbon/{min}/{max}")]
    [HttpPost]
    public ActionResult oc(float min, float max)
    {
        limits.valueLimits["Organic_carbon"] = new float[] { min, max };
        return Ok();
    }
    [Route("Turbidity/{min}/{max}")]
    [HttpPost]
    public ActionResult tu(float min, float max)
    {
        limits.valueLimits["Turbidity"] = new float[] { min, max };
        return Ok();
    }
    [Route("restartLimits")]
    [HttpGet]
    public ActionResult restart()
    {
        limits.valueLimits["ph"] = new float[] { 6.5F, 8.5F };
        limits.valueLimits["Organic_carbon"] = new float[] { 2.0F, 4.0F };
        limits.valueLimits["Turbidity"] = new float[] { 0.1F, 5.0F };
        return Ok();
    }
}


