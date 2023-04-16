using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;

var port = System.Environment.GetEnvironmentVariable("PORT");
if (port is null) {
  port = "8080";
} else if (!int.TryParse(port, out _)) {
  System.Console.Error.WriteLine($"PORT must be a valid integer, or unset. Received \"{port}\"");
  Environment.Exit(1);
}

// Configure the web host.
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
// Configure services for dependency injection.
builder.Services.AddControllers();
builder.Services.AddStaticCompressionMiddleware(options => {
  options.ServingPrefix = new PathString("/static");
  options.WebRoot = builder.Environment.WebRootPath;
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

// Configure app routing (order matters).
app.UseMiddleware<StaticCompressionMiddleware>();
app.UseStaticFiles(new StaticFileOptions {
  RequestPath = "/static",
});

app.MapControllers();

app.Run();
