using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

/// <summary>Serves a precompressed file, if one exists for the requested resource.</summary>
public class StaticCompressionMiddleware : IMiddleware
{
  public record Options {
    public required PathString ServingPrefix = new PathString("/");
    public required String WebRoot = "./wwwroot";
  }

  private const String _BROTLI_ENCODING_HEADER = "br";

  private static readonly FileExtensionContentTypeProvider _contentTypeProvider = new FileExtensionContentTypeProvider();

  private readonly Options _options;

  public StaticCompressionMiddleware(IOptions<Options> options)
  {
    _options = options.Value;
  }

  public async Task InvokeAsync(HttpContext context, RequestDelegate next)
  {
    if (_IsStaticFileRequest(context) && await _TryServeCompressedFile(context))
    {
      return;
    }
    await next.Invoke(context);
  }

  private async Task<bool> _TryServeCompressedFile(HttpContext context) {
    if (!_AcceptsBrotli(context))
    {
      return false;
    }
    var relativePath = context.Request.Path.Value!.Substring(_options.ServingPrefix.Value!.Length);
    string? contentType;
    if (!_contentTypeProvider.TryGetContentType(relativePath, out contentType)
        || !contentType.StartsWith("text/")) {
      return false;
    }
    var compressedPath = Path.Join(_options.WebRoot, relativePath + ".br");
    if (!File.Exists(compressedPath)) {
      return false;
    }
    context.Response.ContentType = contentType;
    context.Response.Headers.ContentEncoding = _BROTLI_ENCODING_HEADER;
    await context.Response.SendFileAsync(compressedPath);
    return true;
  }

  private bool _IsStaticFileRequest(HttpContext context) =>
    context.Request.Path.StartsWithSegments(_options.ServingPrefix);

  private static bool _AcceptsBrotli(HttpContext context) =>
    context.Request.Headers.AcceptEncoding.SelectMany(v => v!.Split(",")).Any(v => v.Trim() == _BROTLI_ENCODING_HEADER);
}

public static class AppServicesExtensions
{
  public static IServiceCollection AddStaticCompressionMiddleware(
      this IServiceCollection services, Action<StaticCompressionMiddleware.Options> configureOptions)
  {
    services.Configure<StaticCompressionMiddleware.Options>(configureOptions);
    services.AddSingleton<StaticCompressionMiddleware>();
    return services;
  }
}
