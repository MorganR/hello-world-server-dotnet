using Microsoft.AspNetCore.Mvc;

[Route("/[controller]")]
/// <summary>Provides math endpoints.</summary>
public class MathController : Controller {
  /// <summary>Computes a convergent series with "n" terms.</summary>
  [HttpGet("power-reciprocals-alt")]
  public ActionResult<string> PowerReciprocalsAlt(int? n) {
    if (n < 0)
    {
      return BadRequest();
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