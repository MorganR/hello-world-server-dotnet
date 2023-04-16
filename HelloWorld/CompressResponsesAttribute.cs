using Microsoft.AspNetCore.Mvc;

/// <summary>An attribute filter to selectively enable response compression.</summary>
public class CompressResponsesAttribute : MiddlewareFilterAttribute
{
  public CompressResponsesAttribute() : base(typeof(CompressResponsesAttribute)) {}

  public void Configure(IApplicationBuilder builder)
  {
    builder.UseResponseCompression();
  }
}