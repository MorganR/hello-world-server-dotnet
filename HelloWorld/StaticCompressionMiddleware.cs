/// <summary>Serves a precompressed file, if one exists for the requested resource.</summary>
public class StaticCompressionMiddleware : IMiddleware
{
  public record Options {
    public PathString ServingPrefix;
    // TODO: indicate that this must be initialized
    public String WebRoot;
  }

  private const String _BROTLI_ENCODING_HEADER = "br";

  private readonly Options _options;

  public StaticCompressionMiddleware(Options options) {
    _options = options;
  }

  public async Task InvokeAsync(HttpContext context, RequestDelegate next)
  {
    if (_IsStaticFileRequest(context) && _AcceptsBrotli(context))
    {
      var relativePath = context.Request.Path.Value.Substring(_options.ServingPrefix.Value!.Length);
      var compressedPath = Path.Join(_options.WebRoot, relativePath + ".br");
      if (File.Exists(compressedPath)) {
        context.Response.ContentType = "text/html;charset=utf-8";
        context.Response.Headers.ContentEncoding = _BROTLI_ENCODING_HEADER;
        await context.Response.SendFileAsync(compressedPath);
        return;
      }
    }
    await next.Invoke(context);
  }

  private bool _IsStaticFileRequest(HttpContext context) =>
    context.Request.Path.StartsWithSegments(_options.ServingPrefix);

  private static bool _AcceptsBrotli(HttpContext context) =>
    context.Request.Headers.AcceptEncoding.SelectMany(v => v!.Split(",")).Any(v => v.Trim() == _BROTLI_ENCODING_HEADER);
}

public static class AppServicesExtensions
{
  public static IServiceCollection AddStaticCompressionMiddleware(
      this IServiceCollection services, StaticCompressionMiddleware.Options options)
  {
    services.AddSingleton<StaticCompressionMiddleware.Options>(options);
    services.AddSingleton<StaticCompressionMiddleware>();
    return services;
  }
}
