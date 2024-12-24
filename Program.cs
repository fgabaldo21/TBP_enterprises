using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.UseHttpsRedirection();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});

foreach (var address in app.Urls)
{
    Console.WriteLine($"Aplikacija je dostupna na: {address}");
}

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("hr-HR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("hr-HR");

app.Run();
