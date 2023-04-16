using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

/// <summary>
/// Terminal middleware that serves a precompressed file, if one exists for the requested resource.
/// <para>This must be added before calling <c>app.UseStaticFiles(...)</c>.</para>
/// </summary>
public class StaticCompressionMiddleware : IMiddleware
{
  public record Options {
    /// <summary>The URL path prefix from which to host static files.</summary>
    public required PathString ServingPrefix = new PathString("/");
    /// <summary>
    /// The local path prefix from which to retrieve static files, i.e. ASP.NET's "web root".
    /// </summary>
    public required string WebRoot = "./wwwroot";
  }

  private const string _BROTLI_ENCODING_HEADER = "br";

  private static readonly FileExtensionContentTypeProvider _contentTypeProvider = new FileExtensionContentTypeProvider();
  private static readonly IReadOnlyCollection<string> _COMPRESSIBLE_MIME_TYPES = new List<string> {
    "application/json",
    "application/ld+json",
    "application/xml",
    "image/svg+xml",
  };

  private readonly Options _options;

  public StaticCompressionMiddleware(IOptions<Options> options)
  {
    _options = options.Value;
  }

  public async Task InvokeAsync(HttpContext context, RequestDelegate next)
  {
    if (_IsStaticFileRequest(context) && await _TryServeCompressedFile(context))
    {
      // Exit immediately if a precompressed file was successfully served.
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
        || !_IsCompressibleContentType(contentType)) {
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

  private static bool _IsCompressibleContentType(string contentType) =>
    contentType.StartsWith("text/") || _COMPRESSIBLE_MIME_TYPES.Contains(contentType);
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
