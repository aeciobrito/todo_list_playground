using Microsoft.AspNetCore.Mvc;

namespace TodoList.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MathController : ControllerBase
{
    [HttpGet("sum/{n1}/{n2}")]
    public ActionResult<double> Sum(double n1, double n2)
    {
        return Ok(n1 + n2);
    }

    [HttpGet("sub/{n1}/{n2}")]
    public ActionResult<double> Subtract(double n1, double n2)
    {
        return Ok(n1 - n2);
    }

    [HttpGet("mult/{n1}/{n2}")]
    public ActionResult<double> Multiply(double n1, double n2)
    {
        return Ok(n1 * n2);
    }

    [HttpGet("div/{n1}/{n2}")]
    public ActionResult<double> Divide(double n1, double n2)
    {
        if (n2 == 0)
        {
            return BadRequest("Não é possível dividir por zero.");
        }
        return Ok(n1 / n2);
    }
}