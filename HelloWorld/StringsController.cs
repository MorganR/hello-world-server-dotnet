using System.Text;
using Microsoft.AspNetCore.Mvc;

[CompressResponses]
[Route("/[controller]")]
public class StringsController : Controller {
  private const int MAX_NAME_LENGTH = 500;
  [HttpGet("hello")]
  public ActionResult<String> Hello(String? name) {
    if (name is null || name == "")
    {
      return "Hello, world!";
    }
    if (name.Length > MAX_NAME_LENGTH)
    {
      return new BadRequestResult();
    }
    return $"Hello, {name}!";
  }

  [HttpGet("async-hello")]
  public async Task<ActionResult<String>> AsyncHello() {
    await Task.Delay(TimeSpan.FromMilliseconds(15));
    return "Hello, world!";
  }

  [HttpGet("lines")]
  public ActionResult<String> Lines(int? n) {
    int trueN = n ?? 0;
    if (trueN < 0) {
      return new BadRequestResult();
    }
    var result = new StringBuilder("<ol>\n");
    for (int i = 1; i <= trueN; i++) {
      result.AppendLine($"  <li>Item number: {i}</li>");
    }
    result.Append("</ol>");
    // TODO: Need to return this as text/html.
    return result.ToString();
  }
}