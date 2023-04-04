using Microsoft.AspNetCore.Mvc;

public class CompressResponsesAttribute : MiddlewareFilterAttribute
{
  public CompressResponsesAttribute() : base(typeof(CompressResponsesAttribute)) {}

  public void Configure(IApplicationBuilder builder)
  {
    builder.UseResponseCompression();
  }
}