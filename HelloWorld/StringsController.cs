using Microsoft.AspNetCore.Mvc;

public class StringsController : Controller {
  public ActionResult<String> Hello() {
    return "Hello, world!";
  }

  public async Task<ActionResult<String>> AsyncHello() {
    await Task.Delay(TimeSpan.FromMilliseconds(15));
    return "Hello, world!";
  }
}