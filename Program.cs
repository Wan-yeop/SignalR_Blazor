using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using MySignalRCoreApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true; // Add this for detailed error messages
    options.MaximumReceiveMessageSize = 102400; // Optional: Increase message size limit if needed
});


var app = builder.Build();

// Then configure middleware in correct order
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

//app.MapRazorPages();
//app.MapHub<ChatHub>("/chatHub");

// Blazor Server Setting
app.MapBlazorHub();
app.MapHub<ChatHub>("/chatHub");
app.MapFallbackToPage("/_Host");

app.Run();
