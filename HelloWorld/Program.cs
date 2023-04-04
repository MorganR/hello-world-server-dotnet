using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddStaticCompressionMiddleware(new StaticCompressionMiddleware.Options {
  ServingPrefix = new PathString("/static"),
  WebRoot = builder.Environment.WebRootPath,
});
builder.Services.AddResponseCompression(options =>
{
  options.EnableForHttps = true;
  options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
  options.Level = CompressionLevel.Optimal;
});

var app = builder.Build();

app.UseMiddleware<StaticCompressionMiddleware>();
app.UseStaticFiles(new StaticFileOptions {
  // TODO: Enable serving pre-compressed.
  RequestPath = "/static",
});

app.MapControllers();

app.Run();
