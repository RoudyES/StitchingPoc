using Microsoft.AspNetCore.ResponseCompression;
using StitchingPoc.Server.Hubs;
using StitchingPoc.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<MainService>();
builder.Services.AddRazorPages();
builder.Services.AddCors();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapHub<SignalHub>("/signalhub");
app.MapFallbackToFile("index.html");

app.Run();
