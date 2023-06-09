using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc;

[CompressResponses]
[Route("/[controller]")]
/// <sumary>Provides simple string related endpoints.</summary>.
public class StringsController : Controller {
  private const int MAX_NAME_LENGTH = 500;
  [HttpGet("hello")]
  /// <summary>Greets the subject of the "name" query parameter, else the world.</summary>
  public ActionResult<string> Hello(string? name) {
    if (name is null || name == "")
    {
      return "Hello, world!";
    }
    if (name.Length > MAX_NAME_LENGTH)
    {
      return BadRequest();
    }
    return $"Hello, {name}!";
  }

  [HttpGet("async-hello")]
  /// <summary>Responds with a greeting after a short delay.</summary>
  public async Task<ActionResult<string>> AsyncHello() {
    await Task.Delay(TimeSpan.FromMilliseconds(15));
    return "Hello, world!";
  }

  [HttpGet("lines")]
  /// <summary>Responds with an ordered list of "n" items.</summary>
  public ActionResult Lines(int? n) {
    int trueN = n ?? 0;
    if (trueN < 0) {
      return BadRequest();
    }
    var result = new StringBuilder("<ol>\n");
    for (int i = 1; i <= trueN; i++) {
      result.AppendLine($"  <li>Item number: {i}</li>");
    }
    result.Append("</ol>");
    return base.Content(result.ToString(), MediaTypeNames.Text.Html, Encoding.UTF8);
  }
}