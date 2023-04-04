using Microsoft.AspNetCore.Mvc;

[Route("/[controller]")]
public class MathController : Controller {
  [HttpGet("power-reciprocals-alt")]
  public ActionResult<String> PowerReciprocalsAlt(int? n) {
    if (n < 0)
    {
      return new BadRequestResult();
    }
    int trueN = n ?? 0;
    double result = 0;
    double power = 0.5;
    while (trueN-- > 0)
    {
      power *= 2;
      result += 1 / power;

      if (trueN-- > 0) {
        power *= 2;
        result -= 1 / power;
      }
    }

    return result.ToString();
  }
}